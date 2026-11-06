using System.Threading;
using Contexts.Gameplay;
using Cysharp.Threading.Tasks;
using Persistance.Features.User.Features.JsonUser;
using Persistence.Features.AuthProvider;
using Template.Features.DataStorage;
using Template.Features.GameFlow.Scripts;
using Template.Features.SceneLoader;
using Template.Gameplay;

namespace Template.Features.GameStateMachine
{
    public class AuthState : IState
    {
        private const string SceneName = "Scene_Auth";

        private readonly IGameStateMachine _stateMachine;
        private readonly IAuthProvider _authProvider;
        private readonly ISceneLoader _sceneLoader;
        private readonly IFeatureInitializer _featureInitializer;
        private readonly IDataProvider<PlayerData> _playerDataProvider;

        private LoadedScene _scene;

        public AuthState(
            IGameStateMachine stateMachine,
            IAuthProvider authProvider,
            ISceneLoader sceneLoader,
            IFeatureInitializer featureInitializer, IDataProvider<PlayerData> playerDataProvider)
        {
            _stateMachine = stateMachine;
            _authProvider = authProvider;
            _sceneLoader = sceneLoader;
            _featureInitializer = featureInitializer;
            _playerDataProvider = playerDataProvider;
        }

        public async UniTask Enter(CancellationToken lifetime)
        {
            if (!_authProvider.IsFirebaseInitialized)
                await _authProvider.InitAsync().AttachExternalCancellation(lifetime);

            if (!_authProvider.TryGetUser(out _))
            {
                await RunSignIn(lifetime);
            }

            await LoadPlayerData(lifetime);

            _stateMachine.Enter<GameplayState, GamePlayStatePayloads>(new(1));
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

        private async UniTask LoadPlayerData(CancellationToken lifetime)
        {
            bool exists = await _playerDataProvider.Exists();

            if (exists)
                await _playerDataProvider.TryLoad();
            else
                _playerDataProvider.Reset();
        }
    }
}