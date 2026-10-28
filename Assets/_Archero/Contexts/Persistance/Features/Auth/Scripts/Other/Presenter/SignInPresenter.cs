using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Persistence.Features.Auth;
using Template.Features.ScreenNavigator;
using Template.Scripts.DataHolders;
using Template.Utilities.Primitives;
using UnityEngine;

namespace Persistance.Features.Auth
{
    public class SignInPresenter : IScreenPresenter<SignInScreenPayload>
    {
        private readonly ISignInView _view;
        private readonly IAuthService _authService;

        private bool _disposed;

        public SignInPresenter(ISignInView view, IAuthService authService)
        {
            _view = view;
            _authService = authService;

            _view.LoginSubmitted += OnLoginSubmitted;
            _view.RegisterSubmitted += OnRegisterSubmitted;
        }

        public UniTask Activate(SignInScreenPayload payload)
        {
            _view.Reset();
            return UniTask.CompletedTask;
        }

        private void OnLoginSubmitted(LoginForm form) => LoginAsync(form).Forget();

        private void OnRegisterSubmitted(RegisterForm form) => RegisterAsync(form).Forget();

        private async UniTaskVoid LoginAsync(LoginForm form)
        {
            _view.ClearErrors();

            if (!SignInValidator.ValidateEmail(form.Email, out string emailError))
            {
                _view.ShowError(AuthForm.Login, emailError);
                return;
            }

            if (!SignInValidator.ValidatePassword(form.Password, out string passwordError))
            {
                _view.ShowError(AuthForm.Login, passwordError);
                return;
            }

            _view.SetBusy(AuthForm.Login, true);

            try
            {
                Result loginResult = await _authService.SignInEmailAsync(
                    Email.Create(form.Email).Value,
                    Password.Create(form.Password).Value);

                if (_disposed)
                    return;

                if (loginResult.IsFailure)
                {
                    Debug.LogError(loginResult.Error);
                    _view.ShowError(AuthForm.Login, loginResult.Error.Message);
                }
                else
                {
                    Debug.Log("Login successful");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                if (!_disposed)
                    _view.ShowError(AuthForm.Login, "Error connection");
            }
            finally
            {
                if (!_disposed)
                    _view.SetBusy(AuthForm.Login, false);
            }
        }

        private async UniTaskVoid RegisterAsync(RegisterForm form)
        {
            _view.ClearErrors();

            if (!SignInValidator.ValidateEmail(form.Email, out string emailError))
            {
                _view.ShowError(AuthForm.Register, emailError);
                return;
            }

            if (!SignInValidator.ValidatePassword(form.Password, out string passwordError))
            {
                _view.ShowError(AuthForm.Register, passwordError);
                return;
            }

            if (!SignInValidator.ValidateConfirmPassword(form.Password, form.ConfirmPassword,
                    out string confirmError))
            {
                _view.ShowError(AuthForm.Register, confirmError);
                return;
            }

            if (!SignInValidator.ValidateAge(form.Age, out int age, out string ageError))
            {
                _view.ShowError(AuthForm.Register, ageError);
                return;
            }

            string userName = form.Email;

            _view.SetBusy(AuthForm.Register, true);

            try
            {
                Result registerResult = await _authService.RegisterEmailAsync(
                    Email.Create(form.Email).Value,
                    UserName.Create(userName).Value,
                    Password.Create(form.Password).Value,
                    age);

                if (_disposed)
                    return;

                if (registerResult.IsFailure)
                {
                    _view.ShowError(AuthForm.Register, registerResult.Error.Message);
                }
                else
                {
                    Debug.Log("Registration successful");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                if (!_disposed)
                    _view.ShowError(AuthForm.Register, "Error connection");
            }
            finally
            {
                if (!_disposed)
                    _view.SetBusy(AuthForm.Register, false);
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _view.LoginSubmitted -= OnLoginSubmitted;
            _view.RegisterSubmitted -= OnRegisterSubmitted;
        }
    }
}