using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public class ScreenNavigator : IScreenNavigator
    {
        private readonly IMainNavigationItem _mainNavigationItem;

        private readonly Dictionary<Type, List<BaseNavigationItem>> _pathCache = new();

        private readonly List<BaseNavigationItem> _currentPath = new();
        private readonly ReadOnlyCollection<BaseNavigationItem> _currentPathView;

        private UniTaskCompletionSource _lastOperation;
        private int _pendingOperations;

        public event Action<IReadOnlyList<BaseNavigationItem>> PathChanged;
        public event Action<Exception> NavigationFailed;

        public ScreenNavigator(IMainNavigationItem mainNavigationItem)
        {
            _mainNavigationItem = mainNavigationItem;
            _currentPathView = new ReadOnlyCollection<BaseNavigationItem>(_currentPath);
        }

        public IReadOnlyList<BaseNavigationItem> CurrentPath => _currentPathView;

        public bool IsNavigating => _pendingOperations > 0;

        public void Init()
        {
            _mainNavigationItem.Init();

            if (_mainNavigationItem is BaseNavigationItem root)
                ValidateTree(root, new HashSet<Type>());
        }

        public UniTask NavigateTo<TItem, TPayload>(TPayload payload, CancellationToken cancellationToken = default)
            where TItem : BaseNavigationItem<TPayload>
            where TPayload : BaseNavigationItemPayload
        {
            return RunExclusive(() => Navigate<TItem, TPayload>(payload, cancellationToken), cancellationToken);
        }

        public UniTask NavigateTo<TItem>(CancellationToken cancellationToken = default)
            where TItem : BaseNavigationItem<EmptyNavigationItemPayload>
        {
            return NavigateTo<TItem, EmptyNavigationItemPayload>(new EmptyNavigationItemPayload(), cancellationToken);
        }

        private async UniTask Navigate<TItem, TPayload>(TPayload payload, CancellationToken cancellationToken)
            where TItem : BaseNavigationItem<TPayload>
            where TPayload : BaseNavigationItemPayload
        {
            List<BaseNavigationItem> target = GetPath<TItem>();

            int common = CommonPrefixLength(_currentPath, target);

            List<BaseNavigationItem> closing = new(_currentPath.Count - common);
            for (int i = _currentPath.Count - 1; i >= common; i--)
                closing.Add(_currentPath[i]);

            List<BaseNavigationItem> opening = target.GetRange(common, target.Count - common);

            await PrepareOpening(opening, target[target.Count - 1], payload, cancellationToken);

            bool openingStarted = false;
            try
            {
                foreach (BaseNavigationItem item in closing)
                {
                    try
                    {
                        await item.Deactivate(new NavigationItemClosePayload(opening));
                    }
                    finally
                    {
                        _currentPath.RemoveAt(_currentPath.Count - 1);
                    }
                }

                if (opening.Count > 0)
                {
                    openingStarted = true;
                    await opening[0].MoveTo<TItem, TPayload>(payload, new NavigationItemOpenPayload(closing));
                    _currentPath.AddRange(opening);
                }
                else
                {
                    await target[target.Count - 1].Refresh(payload);
                }
            }
            catch
            {
                if (openingStarted)
                    await RollbackOpened(opening);

                AbortPrepared(opening);

                RaisePathChanged();
                throw;
            }

            RaisePathChanged();
        }

        private async UniTask PrepareOpening(
            List<BaseNavigationItem> opening,
            BaseNavigationItem targetItem,
            BaseNavigationItemPayload payload,
            CancellationToken cancellationToken)
        {
            try
            {
                foreach (BaseNavigationItem item in opening)
                {
                    if (item.IsPrepareBarrier)
                        break;

                    cancellationToken.ThrowIfCancellationRequested();
                    await item.Prepare(payload, ReferenceEquals(item, targetItem), cancellationToken);
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
            catch
            {
                AbortPrepared(opening);
                throw;
            }
        }

        private static void AbortPrepared(List<BaseNavigationItem> opening)
        {
            foreach (BaseNavigationItem item in opening)
            {
                try
                {
                    item.AbortPrepare();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private async UniTask RollbackOpened(List<BaseNavigationItem> opening)
        {
            for (int i = opening.Count - 1; i >= 0; i--)
            {
                BaseNavigationItem item = opening[i];
                if (!item.IsActive)
                    continue;

                try
                {
                    await item.Deactivate(new NavigationItemClosePayload(new List<BaseNavigationItem>()));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private List<BaseNavigationItem> GetPath<TItem>() where TItem : BaseNavigationItem
        {
            Type type = typeof(TItem);
            if (_pathCache.TryGetValue(type, out List<BaseNavigationItem> cached))
                return cached;

            List<BaseNavigationItem> path = _mainNavigationItem.GetItemPath<TItem>();
            if (path == null)
                throw new InvalidOperationException($"{type.Name} is not registered in the navigation tree.");

            _pathCache[type] = path;
            return path;
        }

        private static int CommonPrefixLength(List<BaseNavigationItem> a, List<BaseNavigationItem> b)
        {
            int min = Math.Min(a.Count, b.Count);

            int i = 0;
            while (i < min && ReferenceEquals(a[i], b[i]))
                i++;

            return i;
        }

        private static void ValidateTree(BaseNavigationItem item, HashSet<Type> seen)
        {
            if (!seen.Add(item.GetType()))
                Debug.LogError($"[{nameof(ScreenNavigator)}] {item.GetType().Name} appears in the navigation tree " +
                               "more than once: navigation to it would be ambiguous.");

            foreach (BaseNavigationItem child in item.Children)
                ValidateTree(child, seen);
        }

        private async UniTask RunExclusive(Func<UniTask> operation, CancellationToken cancellationToken)
        {
            UniTaskCompletionSource previous = _lastOperation;
            UniTaskCompletionSource current = new UniTaskCompletionSource();
            _lastOperation = current;
            _pendingOperations++;

            bool hadTurn = false;
            try
            {
                if (previous != null)
                    await previous.Task.AttachExternalCancellation(cancellationToken);

                hadTurn = true;
                cancellationToken.ThrowIfCancellationRequested();

                await operation();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                RaiseNavigationFailed(e);
                throw;
            }
            finally
            {
                _pendingOperations--;

                if (hadTurn || previous == null)
                    current.TrySetResult();
                else
                    // Отменились ещё в очереди: следующая операция не должна стартовать раньше предыдущей.
                    CompleteAfter(previous, current).Forget();
            }
        }

        private static async UniTaskVoid CompleteAfter(UniTaskCompletionSource previous,
            UniTaskCompletionSource current)
        {
            await previous.Task;
            current.TrySetResult();
        }

        private void RaisePathChanged()
        {
            try
            {
                PathChanged?.Invoke(_currentPathView);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void RaiseNavigationFailed(Exception exception)
        {
            try
            {
                NavigationFailed?.Invoke(exception);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}