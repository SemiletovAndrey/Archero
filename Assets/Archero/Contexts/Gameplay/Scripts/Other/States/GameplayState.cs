using System.Threading;
using Contexts.Gameplay;
using Cysharp.Threading.Tasks;
using Template.Features.GameFlow.Scripts;
using Template.Features.GameStateMachine;
using Template.Features.NewInputFeature;
using Template.Features.NewInputFeature.Scripts.Example;
using Template.Features.SceneLoader;
using Template.Features.UILoader;

namespace Template.Gameplay
{
    public class GameplayState : IPayloadState<GamePlayStatePayloads>
    {
        private const string SceneName = "Scene_Gameplay";

        private readonly ISceneLoader _sceneLoader;
        private readonly IFeatureInitializer _featureInitializer;
        private readonly IInputProviderManager _inputProviderManager;
        private readonly ILoadingSystem _loadingSystem;

        private LoadedScene _scene;

        public GameplayState(
            ISceneLoader sceneLoader,
            IFeatureInitializer featureInitializer, IInputProviderManager inputProviderManager,
            ILoadingSystem loadingSystem)
        {
            _sceneLoader = sceneLoader;
            _featureInitializer = featureInitializer;
            _inputProviderManager = inputProviderManager;
            _loadingSystem = loadingSystem;
        }
        
        public async UniTask Enter(GamePlayStatePayloads payload, CancellationToken lifetime)
        {
            await _loadingSystem.Show();

            await LoadScene(lifetime);

            HandleInputProvider();
        }

        private void HandleInputProvider()
        {
            _inputProviderManager.CreateProvider<ArcheroInputProvider>();
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
                await _featureInitializer.DeinitAll(scene.Features);
            }
            finally
            {
                await _sceneLoader.Release(scene);
            }
        }
    }
}