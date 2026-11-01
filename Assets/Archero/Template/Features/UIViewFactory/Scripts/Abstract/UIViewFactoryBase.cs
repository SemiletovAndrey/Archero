using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Template.Features.UIViewFactory
{
    public abstract class UIViewFactoryBase<TView, TEntry> : IUIViewFactory<TView, TEntry>, IDisposable
        where TView : Component
        where TEntry : class
    {
        private const int NoSortingOrder = -1;

        private sealed class ViewInstance
        {
            public readonly TEntry Entry;
            public int SortingOrder = NoSortingOrder;

            public ViewInstance(TEntry entry)
            {
                Entry = entry;
            }
        }

        private Transform _parent;
        private bool _disposed;

        private readonly IClassFactory _classFactory;
        private readonly IUILayersService _layersService;

        private readonly Dictionary<TView, ViewInstance> _instances = new();

        private readonly Dictionary<int, TView> _pool = new();

        protected UIViewFactoryBase(IClassFactory classFactory, IUILayersService layersService)
        {
            _classFactory = classFactory;
            _layersService = layersService;
        }

        #region Implemented by

        protected abstract UILayerType Layer { get; }

        protected abstract int GetKey(TEntry entry);

        protected abstract bool IsPooled(TEntry entry);

        protected abstract bool HasSource(TEntry entry);

        protected abstract string Describe(TEntry entry);

        protected abstract UniTask<GameObject> AcquirePrefab(TEntry entry);

        protected abstract void ReleasePrefab(TEntry entry);

        protected virtual void OnViewCreated(TView view, TEntry entry)
        {
        }

        protected virtual void OnViewReused(TView view, TEntry entry)
        {
        }

        protected virtual void OnDispose()
        {
        }

        #endregion

        public void Init(Transform parent)
        {
            _parent = parent;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            
            _pool.Clear();

            foreach (KeyValuePair<TView, ViewInstance> pair in new List<KeyValuePair<TView, ViewInstance>>(_instances))
            {
                ReleaseSortingOrder(pair.Value);

                if (!IsDestroyed(pair.Key))
                    Object.Destroy(pair.Key.gameObject);

                ReleasePrefab(pair.Value.Entry);
            }

            _instances.Clear();

            OnDispose();
        }

        public virtual UniTask Preload(TEntry entry, CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }

        public virtual async UniTask Prewarm(TEntry entry, CancellationToken cancellationToken = default)
        {
            EnsureSource(entry);
            cancellationToken.ThrowIfCancellationRequested();

            int key = GetKey(entry);
            if (!IsPooled(entry) || _pool.ContainsKey(key))
                return;

            GameObject prefab = await AcquirePrefabOrCancel(entry, cancellationToken);

            if (_pool.ContainsKey(key))
            {
                ReleasePrefab(entry);
                return;
            }

            TView view = null;
            try
            {
                view = InstantiateView<TView>(prefab, entry, ResolveParent(null));
                _instances[view] = new ViewInstance(entry);

                OnViewCreated(view, entry);

                view.gameObject.SetActive(false);
                _pool[key] = view;
            }
            catch
            {
                if (view != null)
                {
                    _instances.Remove(view);

                    if (!IsDestroyed(view))
                        Object.Destroy(view.gameObject);
                }

                ReleasePrefab(entry);
                throw;
            }
        }

        public virtual void Unload(TEntry entry)
        {
            if (entry == null || !_pool.TryGetValue(GetKey(entry), out TView pooled))
                return;

            _pool.Remove(GetKey(entry));

            if (_instances.TryGetValue(pooled, out ViewInstance info))
                DisposeInstance(pooled, info);
            else if (!IsDestroyed(pooled))
                Object.Destroy(pooled.gameObject);
        }

        public async UniTask<T> Create<T>(TEntry entry, Transform parent = null,
            CancellationToken cancellationToken = default) where T : TView
        {
            Transform target = ResolveParent(parent);
            EnsureSource(entry);
            cancellationToken.ThrowIfCancellationRequested();

            if (IsPooled(entry) && TryTakeFromPool(entry, target, out T pooled))
                return pooled;

            GameObject prefab = await AcquirePrefabOrCancel(entry, cancellationToken);

            T view = null;
            ViewInstance info = null;
            try
            {
                view = InstantiateView<T>(prefab, entry, target);

                info = new ViewInstance(entry);
                _instances[view] = info;
                info.SortingOrder = AssignSortingOrder(view);

                OnViewCreated(view, entry);
                return view;
            }
            catch
            {
                if (info != null)
                    ReleaseSortingOrder(info);

                if (view != null)
                {
                    _instances.Remove(view);

                    if (!IsDestroyed(view))
                        Object.Destroy(view.gameObject);
                }

                ReleasePrefab(entry);
                throw;
            }
        }

        public void Release(TView view)
        {
            if (ReferenceEquals(view, null))
                return;

            if (!_instances.TryGetValue(view, out ViewInstance info))
            {
                if (!IsDestroyed(view))
                    Object.Destroy(view.gameObject);

                return;
            }

            ReleaseSortingOrder(info);

            int key = GetKey(info.Entry);
            if (IsPooled(info.Entry) && !IsDestroyed(view) && !_pool.ContainsKey(key))
            {
                view.gameObject.SetActive(false);
                _pool[key] = view;
                return;
            }

            DisposeInstance(view, info);
        }

        #region Cancellation

        private async UniTask<GameObject> AcquirePrefabOrCancel(TEntry entry, CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled)
                return await AcquirePrefab(entry);

            UniTask<GameObject> acquire = AcquirePrefab(entry).Preserve();

            GameObject prefab;
            try
            {
                prefab = await acquire.AttachExternalCancellation(cancellationToken);
            }
            catch (System.OperationCanceledException)
            {
                ReleaseWhenLoaded(acquire, entry).Forget();
                throw;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                ReleasePrefab(entry);
                cancellationToken.ThrowIfCancellationRequested();
            }

            return prefab;
        }

        private async UniTaskVoid ReleaseWhenLoaded(UniTask<GameObject> acquire, TEntry entry)
        {
            try
            {
                await acquire;
            }
            catch
            {
                return;
            }

            ReleasePrefab(entry);
        }

        #endregion

        #region Instances

        private T InstantiateView<T>(GameObject prefab, TEntry entry, Transform parent) where T : TView
        {
            TView instance = _classFactory.CreateMonoBehaviour<TView>(prefab, parent);
            if (IsDestroyed(instance))
                throw new System.InvalidOperationException(
                    $"{Describe(entry)}: failed to create instance from prefab '{prefab.name}'.");

            if (!instance.TryGetComponent(out T view))
            {
                Object.Destroy(instance.gameObject);
                throw new System.InvalidOperationException(
                    $"Prefab '{prefab.name}' ({Describe(entry)}) has no component {typeof(T).Name}.");
            }

            if (instance.GetComponent<GraphicRaycaster>() == null)
                Debug.LogWarning($"[{GetType().Name}] Prefab '{prefab.name}' ({Describe(entry)}) " +
                                 "has no GraphicRaycaster on the root object - it will not receive clicks.");

            return view;
        }

        private bool TryTakeFromPool<T>(TEntry entry, Transform parent, out T view) where T : TView
        {
            view = null;

            int key = GetKey(entry);
            if (!_pool.TryGetValue(key, out TView pooled))
                return false;

            _pool.Remove(key);

            if (!_instances.TryGetValue(pooled, out ViewInstance info))
            {
                if (!IsDestroyed(pooled))
                    Object.Destroy(pooled.gameObject);

                return false;
            }

            if (IsDestroyed(pooled) || pooled is not T typed)
            {
                DisposeInstance(pooled, info);
                return false;
            }

            if (pooled.transform.parent != parent)
                pooled.transform.SetParent(parent, false);

            pooled.gameObject.SetActive(true);
            pooled.transform.SetAsLastSibling();
            info.SortingOrder = AssignSortingOrder(pooled);

            OnViewReused(pooled, info.Entry);

            view = typed;
            return true;
        }

        private void DisposeInstance(TView view, ViewInstance info)
        {
            _instances.Remove(view);

            if (!IsDestroyed(view))
                Object.Destroy(view.gameObject);

            ReleasePrefab(info.Entry);
        }

        private int AssignSortingOrder(TView view)
        {
            if (!view.TryGetComponent(out Canvas canvas))
            {
                Debug.LogWarning($"[{GetType().Name}] '{view.name}' has no Canvas on the root object - " +
                                 "sorting order was not applied.");
                return NoSortingOrder;
            }

            int order = _layersService.GetNextSortingOrder(Layer);
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
            return order;
        }

        private void ReleaseSortingOrder(ViewInstance info)
        {
            if (info.SortingOrder == NoSortingOrder)
                return;

            _layersService.ReleaseSortingOrder(Layer, info.SortingOrder);
            info.SortingOrder = NoSortingOrder;
        }

        #endregion

        protected void EnsureSource(TEntry entry)
        {
            if (entry == null)
                throw new System.ArgumentNullException(nameof(entry));

            if (!HasSource(entry))
                throw new System.InvalidOperationException($"{Describe(entry)}: prefab source is not assigned.");
        }

        private Transform ResolveParent(Transform parent)
        {
            Transform target = parent != null ? parent : _parent;
            if (target == null)
                throw new System.InvalidOperationException(
                    $"{GetType().Name} is not initialized. Call Init(parent) first or pass a parent to Create.");

            return target;
        }

        protected static bool IsDestroyed(Object unityObject)
        {
            return unityObject == null;
        }
    }
}