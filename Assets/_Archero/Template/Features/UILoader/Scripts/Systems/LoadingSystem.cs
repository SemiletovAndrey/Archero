using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.UILoader
{
    public class LoadingSystem : ILoadingSystem
    {
        private readonly ILoaderView _view;

        // Сколько запросов держат загрузку сейчас.
        private int _users;

        // Вью показана (анимация появления закончилась) и еще не скрыта.
        private bool _isShown;
        private float _shownAt;

        // Завершается, когда вью показана.
        private UniTaskCompletionSource _shownSignal = new();

        // Пока идет приведение вью к нужному состоянию, здесь лежит сигнал его завершения.
        private UniTaskCompletionSource _settled;

        public LoadingSystem(ILoaderView view)
        {
            _view = view;
        }

        public bool IsVisible => _isShown;

        public async UniTask<ILoadingHandle> Show(string message = null)
        {
            _users++;
            Handle handle = new Handle(this);

            if (message != null)
                _view.SetMessage(message);

            if (!_isShown)
            {
                EnsureReconciling().Forget();
                await _shownSignal.Task;
            }

            return handle;
        }

        public async UniTask Run(Func<UniTask> work, string message = null)
        {
            ILoadingHandle handle = await Show(message);
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
            ILoadingHandle handle = await Show(message);
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

        /// <summary>
        /// Приведение вью к нужному состоянию выполняется одним циклом, поэтому анимации не накладываются друг на друга:
        /// пока идет одна, новые запросы только меняют счетчик, а цикл после нее смотрит на актуальное значение.
        /// </summary>
        private UniTask EnsureReconciling()
        {
            // Цикл может закончиться синхронно, поэтому возвращаем локальный сигнал, а не поле.
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
                        await SafeShow();

                        _isShown = true;
                        _shownAt = Time.unscaledTime;
                        _shownSignal.TrySetResult();
                    }
                    else
                    {
                        // Не скрываем раньше минимального времени: загрузка не должна мигать.
                        float remaining = _view.MinVisibleTime - (Time.unscaledTime - _shownAt);
                        if (remaining > 0f)
                            await UniTask.Delay(TimeSpan.FromSeconds(remaining), ignoreTimeScale: true);

                        // За время ожидания загрузку запросили снова: остаемся показанными.
                        if (_users > 0)
                            continue;

                        await SafeHide();

                        _isShown = false;
                        _shownSignal = new UniTaskCompletionSource();
                    }
                }
            }
            finally
            {
                UniTaskCompletionSource settled = _settled;
                _settled = null;
                settled?.TrySetResult();
            }
        }

        // Ошибка во вью не должна навсегда оставить загрузку на экране или повесить вызывающих.
        private async UniTask SafeShow()
        {
            try
            {
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
            private bool _released;

            public Handle(LoadingSystem loadingSystem)
            {
                _loadingSystem = loadingSystem;
            }

            public void SetProgress(float progress)
            {
                if (!_released)
                    _loadingSystem._view.SetProgress(progress);
            }

            public void SetMessage(string message)
            {
                if (!_released)
                    _loadingSystem._view.SetMessage(message);
            }

            public UniTask Hide()
            {
                if (_released)
                    return UniTask.CompletedTask;

                _released = true;
                return _loadingSystem.ReleaseUser();
            }

            public void Dispose()
            {
                Hide().Forget();
            }
        }
    }
}