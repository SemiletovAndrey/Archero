using System;
using Cysharp.Threading.Tasks;
using Template.Scripts.Utils;
using UnityEngine;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class SceneSelectorNavigationItem<TPayload> : BaseNavigationItem<TPayload>
        where TPayload : BaseNavigationItemPayload
    {
        private ZenjectSceneLoader _sceneLoader;

        [Inject]
        private void Construct(ZenjectSceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public event Func<TPayload, UniTask> AfterSceneOpened;
        public event Action<TPayload> AfterSceneInitialized;

        public override async UniTask MoveTo<TItem, TItemData>(TItemData data, NavigationItemOpenPayload payload)
        {
            TPayload scenePayload = GetType() == typeof(TItem) ? (TPayload)(object)data : null;
            string sceneName = scenePayload != null ? GetSceneName(scenePayload) : GetDefaultSceneName();

            await OpenLoader(scenePayload);
            try
            {
                await _sceneLoader.LoadSceneAsync(sceneName).ToUniTask();
                await AfterSceneOpened.InvokeAllAsync(scenePayload);
            }
            finally
            {
                await CloseLoaderSafe();
            }

            await base.MoveTo<TItem, TItemData>(data, payload);

            AfterSceneInitialized?.Invoke(scenePayload);
        }

        protected abstract string GetDefaultSceneName();

        protected virtual string GetSceneName(TPayload payload)
        {
            return GetDefaultSceneName();
        }

        protected virtual UniTask OpenLoader(TPayload payload)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask CloseLoader()
        {
            return UniTask.CompletedTask;
        }

        private async UniTask CloseLoaderSafe()
        {
            try
            {
                await CloseLoader();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}