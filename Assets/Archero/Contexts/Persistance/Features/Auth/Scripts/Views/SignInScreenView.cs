using System;
using Cysharp.Threading.Tasks;
using Persistance.Features.Auth;
using Template.Features.ScreenNavigator;
using Template.Features.UIExtensions;
using TMPro;
using UnityEngine;

namespace Persistence.Features.Auth
{
    public class SignInScreenView : ActivatableScreen<SignInScreenPayload>, ISignInView
    {
        [Header("Sign In Container")] [SerializeField]
        private GameObject _signInContainer;

        [SerializeField] private TMP_InputField _loginEmailInputField;
        [SerializeField] private TMP_InputField _loginPasswordInputField;
        [SerializeField] private UIButton _loginButton;
        [SerializeField] private GameObject _errorLoginContainer;
        [SerializeField] private TextMeshProUGUI _errorLoginText;

        [Header("Registration Container")] [SerializeField]
        private GameObject _registrationContainer;

        [SerializeField] private TMP_InputField _registerEmailInputField;
        [SerializeField] private TMP_InputField _registerPasswordInputField;
        [SerializeField] private TMP_InputField _registerConfirmPasswordInputField;
        [SerializeField] private TMP_InputField _registerAgeInputField;
        [SerializeField] private UIButton _registerButton;
        [SerializeField] private GameObject _errorRegisterContainer;
        [SerializeField] private TextMeshProUGUI _errorRegisterText;

        public event Action<LoginForm> LoginSubmitted;
        public event Action<RegisterForm> RegisterSubmitted;

        public override UniTask Activate(SignInScreenPayload data)
        {
            _loginButton.AssignAction(OnLoginClicked);
            _registerButton.AssignAction(OnRegisterClicked);
            return UniTask.CompletedTask;
        }
        
        public void OnSwitchToRegisterClicked()
        {
            ClearAllFields();
            ClearErrors();
            ShowRegistrationContainer();
        }

        public void OnBackToLoginClicked()
        {
            ClearAllFields();
            ClearErrors();
            ShowLoginContainer();
        }

        #region ISignInView

        public void Reset()
        {
            ClearAllFields();
            ClearErrors();
            ShowLoginContainer();
        }

        public void ClearErrors()
        {
            _errorLoginContainer.SetActive(false);
            _errorRegisterContainer.SetActive(false);
            _errorLoginText.text = "";
            _errorRegisterText.text = "";
        }

        public void ShowError(AuthForm form, string message)
        {
            if (form == AuthForm.Login)
            {
                _errorLoginContainer.SetActive(true);
                _errorLoginText.text = message;
            }
            else
            {
                _errorRegisterContainer.SetActive(true);
                _errorRegisterText.text = message;
            }
        }

        public void SetBusy(AuthForm form, bool busy)
        {
            UIButton button = form == AuthForm.Login ? _loginButton : _registerButton;
            button.SetAvailability(!busy);
        }

        #endregion

        private void OnLoginClicked()
        {
            LoginSubmitted?.Invoke(new LoginForm(
                _loginEmailInputField.text.Trim(),
                _loginPasswordInputField.text));
        }

        private void OnRegisterClicked()
        {
            RegisterSubmitted?.Invoke(new RegisterForm(
                _registerEmailInputField.text.Trim(),
                _registerPasswordInputField.text,
                _registerConfirmPasswordInputField.text,
                _registerAgeInputField.text));
        }

        private void ShowLoginContainer()
        {
            _signInContainer.SetActive(true);
            _registrationContainer.SetActive(false);
        }

        private void ShowRegistrationContainer()
        {
            _signInContainer.SetActive(false);
            _registrationContainer.SetActive(true);
        }

        private void ClearAllFields()
        {
            _loginEmailInputField.text = "";
            _loginPasswordInputField.text = "";
            _registerEmailInputField.text = "";
            _registerPasswordInputField.text = "";
            _registerConfirmPasswordInputField.text = "";
            _registerAgeInputField.text = "";
        }
    }
}
