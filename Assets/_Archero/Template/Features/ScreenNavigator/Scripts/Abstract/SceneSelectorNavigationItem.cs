using System;
using Cysharp.Threading.Tasks;
using Template.Features.Popups;
using UnityEngine;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class SceneSelectorNavigationItem<TPayload> : BaseNavigationItem<TPayload> 
        where TPayload : BaseNavigationItemPayload
    {
        private ZenjectSceneLoader _sceneLoader;
        private IPopupSystem _popupSystem;
        
        [Inject]
        private void Construct(ZenjectSceneLoader sceneLoader, IPopupSystem popupSystem)
        {
            _sceneLoader = sceneLoader;
            _popupSystem = popupSystem;
        }

        public event Func<TPayload, UniTask> AfterSceneOpened; 
        public event Action<TPayload> AfterSceneInitialized; 
        
        public override async UniTask MoveTo<TItem, TItemData>(TItemData data, NavigationItemOpenPayload payload)
        {
            TPayload scenePayload = GetType() == typeof(TItem) ? (TPayload)(object)data : null;
            string sceneName = scenePayload != null ? GetSceneName(scenePayload) : GetDefaultSceneName();
            
            await OpenLoader(scenePayload);
            await _sceneLoader.LoadSceneAsync(sceneName).ToUniTask();

            if (AfterSceneOpened != null)
            {
                await AfterSceneOpened.Invoke(scenePayload);
            }
            
            await CloseLoader();
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
    }
}