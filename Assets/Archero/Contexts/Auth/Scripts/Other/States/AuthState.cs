using System.Threading;
using Persistence.Features.AuthProvider;
using Cysharp.Threading.Tasks;
using Persistence.Features.Auth;
using Template.Features.GameFlow.Scripts;
using Template.Features.SceneLoader;
using Template.Features.UILoader;

namespace Template.Features.GameStateMachine
{
    public class AuthState : IState
    {
        private const string SceneName = "Scene_Auth";

        private readonly IGameStateMachine _stateMachine;
        private readonly IAuthProvider _authProvider;
        private readonly IAuthService _authService;
        private readonly ILoadingSystem _loadingSystem;
        private readonly ISceneLoader _sceneLoader;
        private readonly IFeatureInitializer _featureInitializer;

        private LoadedScene _scene;

        public AuthState(
            IGameStateMachine stateMachine,
            IAuthProvider authProvider,
            ILoadingSystem loadingSystem,
            ISceneLoader sceneLoader,
            IFeatureInitializer featureInitializer, IAuthService authService)
        {
            _stateMachine = stateMachine;
            _authProvider = authProvider;
            _loadingSystem = loadingSystem;
            _sceneLoader = sceneLoader;
            _featureInitializer = featureInitializer;
            _authService = authService;
        }

        public async UniTask Enter(CancellationToken lifetime)
        {
            if (!_authProvider.IsFirebaseInitialized)
                await _authProvider.InitAsync().AttachExternalCancellation(lifetime);

            if (!_authProvider.TryGetUser(out _))
            {
                await RunSignIn(lifetime);
            }

            _stateMachine.Enter<MenuState>();
        }

        private async UniTask RunSignIn(CancellationToken lifetime)
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
                return;

            LoadedScene scene = _scene;
            await _featureInitializer.DeinitAll(scene.Features);
            _scene = null;
            await _sceneLoader.Release(scene);
        }
    }
}