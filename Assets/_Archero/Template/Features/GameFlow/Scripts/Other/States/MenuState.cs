using System.Threading;
using Contexts.Persistance.Auth;
using Cysharp.Threading.Tasks;
using Template.Features.GameFlow.Scripts.Other.FeatureInitializer;
using Template.Features.Popups;
using Template.Features.SceneLoader;

namespace Template.Features.GameStateMachine
{
    public class MenuState : IState
    {
        private const string SceneName = "Scene_Menu";
        
        private readonly ISceneLoader _sceneLoader;
        private readonly IFeatureInitializer _featureInitializer;
        private readonly IPopupSystem _popupSystem;
 
        private LoadedScene _scene;
 
        public MenuState(
            ISceneLoader sceneLoader,
            IFeatureInitializer featureInitializer, IPopupSystem popupSystem)
        {
            _sceneLoader = sceneLoader;
            _featureInitializer = featureInitializer;
            _popupSystem = popupSystem;
        }
        
        public async UniTask Enter(CancellationToken lifetime)
        {
            //await _sceneLoader.Load(SceneName, new GameplayPayload(levelId), lifetime);
            _scene = await _sceneLoader.Load(SceneName, lifetime);
            await _featureInitializer.InitAll(_scene.Features, lifetime);

            await _popupSystem.Open<TestPopup, TestPopupPayload>(new TestPopupPayload(1));
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