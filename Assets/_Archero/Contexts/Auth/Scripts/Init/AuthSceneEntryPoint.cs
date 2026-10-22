using System;
using System.Threading;
using Contexts.Persistence.Features.AuthProvider;
using Cysharp.Threading.Tasks;
using Persistence.Features.Auth;
using Template.Features.GameStateMachine;
using Template.Features.ScreenNavigator;

namespace Auth.Scripts.Init
{
    public class AuthSceneEntryPoint : ISceneEntryPoint
    {
        private readonly IScreenNavigator _screenNavigator;
        private readonly IAuthProvider _authProvider;

        public AuthSceneEntryPoint(IScreenNavigator screenNavigator, IAuthProvider authProvider)
        {
            _screenNavigator = screenNavigator;
            _authProvider = authProvider;
        }

        public async UniTask Enter(CancellationToken lifetime)
        {
            await _screenNavigator
                .NavigateTo<SignInScreenItem, SignInScreenPayload>(new SignInScreenPayload())
                .AttachExternalCancellation(lifetime);

            bool signedIn = await _authProvider.WaitForUserAsync().AttachExternalCancellation(lifetime);
            if (!signedIn)
                throw new InvalidOperationException("Sign-in was not completed.");
        }
    }
}