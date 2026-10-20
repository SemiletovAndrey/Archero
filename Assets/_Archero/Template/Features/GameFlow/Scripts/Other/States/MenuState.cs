using System.Threading;
using Contexts.Persistance.Auth;
using Cysharp.Threading.Tasks;
using Template.Features.GameFlow.Scripts;
using Template.Features.Popups;
using Template.Features.SceneLoader;
using UnityEngine;

namespace Template.Features.GameStateMachine
{
    public class MenuState : IState
    {
        private const string SceneName = "Scene_Menu";

        private readonly ISceneLoader _sceneLoader;
        private readonly IFeatureInitializer _featureInitializer;

        private LoadedScene _scene;

        public MenuState(
            ISceneLoader sceneLoader,
            IFeatureInitializer featureInitializer)
        {
            _sceneLoader = sceneLoader;
            _featureInitializer = featureInitializer;
        }

        public async UniTask Enter(CancellationToken lifetime)
        {
            //await _sceneLoader.Load(SceneName, new GameplayPayload(levelId), lifetime);
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
            }
            finally
            {
                await _sceneLoader.Release(scene);
            }
        }
    }
}