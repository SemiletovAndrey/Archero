using System;
using Persistance.Features.Auth;

namespace Persistence.Features.Auth
{
    public interface ISignInView
    {
        public event Action<LoginForm> LoginSubmitted;
        public event Action<RegisterForm> RegisterSubmitted;

        public void Reset();
        public void ClearErrors();
        public void ShowError(AuthForm form, string message);
        public void SetBusy(AuthForm form, bool busy);
    }
}