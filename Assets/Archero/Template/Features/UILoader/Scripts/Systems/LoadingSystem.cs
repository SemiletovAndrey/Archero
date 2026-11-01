using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.UILoader
{
    public class LoadingSystem : ILoadingSystem
    {
        private readonly ILoaderView _view;

        private int _users;

        private bool _globalRequested;

        private int _epoch;

        private bool _isShown;
        private float _shownAt;
        
        private bool _instantShowRequested;

        private UniTaskCompletionSource _shownSignal = new();
        
        private UniTaskCompletionSource _hideInterrupt;

        private UniTaskCompletionSource _settled;

        public LoadingSystem(ILoaderView view)
        {
            _view = view;
        }

        public bool IsVisible => _isShown;

        public UniTask Show(string message = null)
        {
            if (!_globalRequested)
            {
                _globalRequested = true;
                _users++;
            }

            if (message != null)
                _view.SetMessage(message);

            return AwaitShown();
        }
        
        public UniTask ShowWithoutAnimation(string message = null)
        {
            if (!_isShown)
                _instantShowRequested = true;

            return Show(message);
        }

        public UniTask Hide()
        {
            if (!_globalRequested)
                return UniTask.CompletedTask;

            _globalRequested = false;
            return ReleaseUser();
        }

        public UniTask HideAll()
        {
            _epoch++;
            _globalRequested = false;
            _users = 0;

            return EnsureReconciling();
        }

        public async UniTask<ILoadingHandle> Acquire(string message = null)
        {
            _users++;
            Handle handle = new Handle(this, _epoch);

            if (message != null)
                _view.SetMessage(message);

            await AwaitShown();
            return handle;
        }

        private UniTask AwaitShown()
        {
            if (_isShown && _hideInterrupt == null)
                return UniTask.CompletedTask;

            _hideInterrupt?.TrySetResult();
            EnsureReconciling().Forget();
            return _shownSignal.Task;
        }

        public async UniTask Run(Func<UniTask> work, string message = null)
        {
            ILoadingHandle handle = await Acquire(message);
            try
            {
                await work();
            }
            finally
            {
                await handle.Hide();
            }
        }

        public async UniTask<T> Run<T>(Func<UniTask<T>> work, string message = null)
        {
            ILoadingHandle handle = await Acquire(message);
            try
            {
                return await work();
            }
            finally
            {
                await handle.Hide();
            }
        }

        private UniTask ReleaseUser()
        {
            if (_users > 0)
                _users--;

            return EnsureReconciling();
        }

        private UniTask EnsureReconciling()
        {
            if (_settled != null)
                return _settled.Task;

            UniTaskCompletionSource settled = new UniTaskCompletionSource();
            _settled = settled;
            ReconcileLoop().Forget();

            return settled.Task;
        }

        private async UniTask ReconcileLoop()
        {
            try
            {
                while ((_users > 0) != _isShown)
                {
                    if (_users > 0)
                    {
                        bool instant = _instantShowRequested;
                        _instantShowRequested = false;

                        await SafeShow(instant);

                        _isShown = true;
                        _shownAt = Time.unscaledTime;
                        _shownSignal.TrySetResult();
                    }
                    else
                    {
                        float remaining = _view.MinVisibleTime - (Time.unscaledTime - _shownAt);
                        if (remaining > 0f)
                            await UniTask.Delay(TimeSpan.FromSeconds(remaining), ignoreTimeScale: true);

                        if (_users > 0)
                            continue;

                        UniTaskCompletionSource interrupt = new UniTaskCompletionSource();
                        _hideInterrupt = interrupt;
                        _shownSignal = new UniTaskCompletionSource();

                        int finished = await UniTask.WhenAny(SafeHide(), interrupt.Task);

                        _hideInterrupt = null;

                        if (finished == 0)
                        {
                            _isShown = false;
                        }
                        else
                        {
                            await SafeShow(false);

                            _shownAt = Time.unscaledTime;
                            _shownSignal.TrySetResult();
                        }
                    }
                }
            }
            finally
            {
                _instantShowRequested = false;

                UniTaskCompletionSource settled = _settled;
                _settled = null;
                settled?.TrySetResult();
            }
        }

        private async UniTask SafeShow(bool instant)
        {
            try
            {
                if (instant)
                    await _view.ShowWithoutAnimation();
                else
                    await _view.Show();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async UniTask SafeHide()
        {
            try
            {
                await _view.Hide();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private sealed class Handle : ILoadingHandle
        {
            private readonly LoadingSystem _loadingSystem;
            private readonly int _epoch;
            private bool _released;

            public Handle(LoadingSystem loadingSystem, int epoch)
            {
                _loadingSystem = loadingSystem;
                _epoch = epoch;
            }

            private bool IsActive => !_released && _epoch == _loadingSystem._epoch;

            public void SetProgress(float progress)
            {
                if (IsActive)
                    _loadingSystem._view.SetProgress(progress);
            }

            public void SetMessage(string message)
            {
                if (IsActive)
                    _loadingSystem._view.SetMessage(message);
            }

            public UniTask Hide()
            {
                if (_released)
                    return UniTask.CompletedTask;

                bool wasActive = IsActive;
                _released = true;

                return wasActive ? _loadingSystem.ReleaseUser() : UniTask.CompletedTask;
            }

            public void Dispose()
            {
                Hide().Forget();
            }
        }
    }
}