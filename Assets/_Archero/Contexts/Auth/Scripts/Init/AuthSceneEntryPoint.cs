using System.Threading;
using Cysharp.Threading.Tasks;
using Persistence.Features.Auth;
using Template.Features.GameStateMachine;
using Template.Features.ScreenNavigator;
using Template.Features.UILoader;

namespace Auth.Scripts.Init
{
    public class AuthSceneEntryPoint : ISceneEntryPoint
    {
        private readonly IScreenNavigator _screenNavigator;
        private readonly IAuthService _authService;
        private readonly ILoadingSystem _loadingSystem;

        public AuthSceneEntryPoint(IScreenNavigator screenNavigator, IAuthService authService, ILoadingSystem loadingSystem)
        {
            _screenNavigator = screenNavigator;
            _authService = authService;
            _loadingSystem = loadingSystem;
        }

        public async UniTask Enter(CancellationToken lifetime)
        {
            var completion = new UniTaskCompletionSource();
            void OnLoggedIn(UserData _) => completion.TrySetResult();

            _authService.LoggedIn += OnLoggedIn;
            try
            {
                _screenNavigator
                    .NavigateTo<SignInScreenItem, SignInScreenPayload>(new SignInScreenPayload())
                    .AttachExternalCancellation(lifetime).Forget();

                await _loadingSystem.Hide();
                await completion.Task.AttachExternalCancellation(lifetime);
            }
            finally
            {
                _authService.LoggedIn -= OnLoggedIn;
            }
        }
    }
}