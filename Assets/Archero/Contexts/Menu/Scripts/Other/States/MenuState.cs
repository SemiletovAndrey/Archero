using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Persistance.Features.User.Features.JsonUser;
using Persistance.Features.Wallet;
using Template.Features.DataStorage;
using Template.Features.GameFlow.Scripts;
using Template.Features.Popups;
using Template.Features.SceneLoader;
using Template.Features.UILoader;
using UnityEngine;

namespace Template.Features.GameStateMachine
{
    public class MenuState : IState
    {
        private const string SceneName = "Scene_Menu";

        private readonly ISceneLoader _sceneLoader;
        private readonly IFeatureInitializer _featureInitializer;
        private readonly IPopupSystem _popupSystem;
        private readonly ILoadingSystem _loadingSystem;

        private LoadedScene _scene;

        public MenuState(
            ISceneLoader sceneLoader,
            IFeatureInitializer featureInitializer, ILoadingSystem loadingSystem, IPopupSystem popupSystem)
        {
            _sceneLoader = sceneLoader;
            _featureInitializer = featureInitializer;
            _loadingSystem = loadingSystem;
            _popupSystem = popupSystem;
        }

        public async UniTask Enter(CancellationToken lifetime)
        {
            await _loadingSystem.Show();

            await LoadScene(lifetime);
        }

        private async UniTask LoadScene(CancellationToken lifetime)
        {
            _scene = await _sceneLoader.Load(SceneName, lifetime);
            await _featureInitializer.InitAll(_scene.Features, lifetime);

            ISceneEntryPoint entry = _scene.Container.TryResolve<ISceneEntryPoint>();
            if (entry != null)
                await entry.Enter(lifetime);
        }

        public async UniTask Exit()
        {
            if (_scene == null)
            {
                return;
            }

            LoadedScene scene = _scene;
            _scene = null;

            try
            {
                await _popupSystem.CloseAll();
                _popupSystem.UnloadAll();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                await _featureInitializer.DeinitAll(scene.Features);
            }
            finally
            {
                await _sceneLoader.Release(scene);
            }
        }
    }
}