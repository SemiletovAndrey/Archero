using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.GameFlow.Scripts;
using Template.Features.NewInputFeature;
using Template.Features.NewInputFeature.Scripts.Example;
using Template.Features.SceneLoader;

namespace Template.Features.GameStateMachine
{
    public class MenuState : IState
    {
        private const string SceneName = "Scene_Menu";

        private readonly ISceneLoader _sceneLoader;
        private readonly IFeatureInitializer _featureInitializer;
        private readonly IInputProviderManager _inputProviderManager;

        private LoadedScene _scene;

        public MenuState(
            ISceneLoader sceneLoader,
            IFeatureInitializer featureInitializer, IInputProviderManager inputProviderManager)
        {
            _sceneLoader = sceneLoader;
            _featureInitializer = featureInitializer;
            _inputProviderManager = inputProviderManager;
        }

        public async UniTask Enter(CancellationToken lifetime)
        {
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
            }
            finally
            {
                await _sceneLoader.Release(scene);
            }
        }
    }
}