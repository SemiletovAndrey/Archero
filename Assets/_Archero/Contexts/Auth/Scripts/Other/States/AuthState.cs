using System.Threading;
using Contexts.Persistence.Features.AuthProvider;
using Cysharp.Threading.Tasks;
using Template.Features.GameFlow.Scripts;
using Template.Features.SceneLoader;
using Template.Features.UILoader;
using UnityEngine;

namespace Template.Features.GameStateMachine
{
    public class AuthState : IState
    {
        private const string SceneName = "Scene_Auth";

        private readonly IGameStateMachine _stateMachine;
        private readonly IAuthProvider _authProvider;
        private readonly ILoadingSystem _loadingSystem;
        private readonly ISceneLoader _sceneLoader;
        private readonly IFeatureInitializer _featureInitializer;

        private LoadedScene _scene;

        public AuthState(
            IGameStateMachine stateMachine,
            IAuthProvider authProvider,
            ILoadingSystem loadingSystem,
            ISceneLoader sceneLoader,
            IFeatureInitializer featureInitializer)
        {
            _stateMachine = stateMachine;
            _authProvider = authProvider;
            _loadingSystem = loadingSystem;
            _sceneLoader = sceneLoader;
            _featureInitializer = featureInitializer;
        }

        public async UniTask Enter(CancellationToken lifetime)
        {
            Debug.Log("Entering AuthState");
            if (!_authProvider.IsFirebaseInitialized)
                await _authProvider.InitAsync().AttachExternalCancellation(lifetime);

            bool hasUser = _authProvider.TryGetUser(out string userId);
            Debug.Log($"[Auth] hasUser={hasUser}, userId={userId}, isGuest={_authProvider.IsGuest}");
            
            if (!_authProvider.TryGetUser(out _))
            {
                Debug.Log("RUN AUTH AuthState");

                await RunSignIn(lifetime);
            }

            _stateMachine.Enter<MenuState>();
        }

        private async UniTask RunSignIn(CancellationToken lifetime)
        {
            _scene = await _sceneLoader.Load(SceneName, lifetime);
            await _featureInitializer.InitAll(_scene.Features, lifetime);

            await _loadingSystem.Hide();

            ISceneEntryPoint entry = _scene.Container.TryResolve<ISceneEntryPoint>();
            if (entry != null)
                await entry.Enter(lifetime);

            await _loadingSystem.Show();
        }

        public async UniTask Exit()
        {
            if (_scene == null)
                return;

            LoadedScene scene = _scene;
            _scene = null;
            await _sceneLoader.Release(scene);
        }
    }
}