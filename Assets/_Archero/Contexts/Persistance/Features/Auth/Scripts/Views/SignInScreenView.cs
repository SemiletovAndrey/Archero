using System;
using System.Net.Mail;
using Cysharp.Threading.Tasks;
using Template.Features.ScreenNavigator;
using Template.Scripts.DataHolders;
using Template.Scripts.Views;
using Template.Utilities.Primitives;
using TMPro;
using UnityEngine;
using Zenject;

namespace Persistence.Features.Auth
{
    public class SignInScreenView : ActivatableScreen<SignInScreenPayload>
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

        private IAuthService _authService;

        [Inject]
        private void Construct(IAuthService authService)
        {
            _authService = authService;
        }

        public override async UniTask Activate(SignInScreenPayload data)
        {
            ClearAllFields();
            ClearAllErrors();
            ShowLoginContainer();
            SubscribeButtons();
            await UniTask.CompletedTask;
        }

        public override async UniTask Open(NavigationItemOpenPayload payload)
        {
            await UniTask.CompletedTask;
        }

        public override async UniTask Close(NavigationItemClosePayload payload)
        {
            await UniTask.CompletedTask;
        }

        public void OnSwitchToRegisterClicked()
        {
            ClearAllFields();
            ClearAllErrors();
            ShowRegistrationContainer();
        }

        public void OnBackToLoginClicked()
        {
            ClearAllFields();
            ClearAllErrors();
            ShowLoginContainer();
        }

        private void SubscribeButtons()
        {
            _loginButton.AssignAction(OnLoginClicked);
            _registerButton.AssignAction(OnRegisterClicked);
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
        }

        private void ClearAllErrors()
        {
            _errorLoginContainer.SetActive(false);
            _errorRegisterContainer.SetActive(false);
            _errorLoginText.text = "";
            _errorRegisterText.text = "";
        }

        #region Validation

        private bool ValidateEmail(string email, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = "Enter email";
                return false;
            }

            try
            {
                var addr = new MailAddress(email);
                if (addr.Address == email)
                {
                    errorMessage = null;
                    return true;
                }
            }
            catch
            {
                /* fall through */
            }

            errorMessage = "Incorrect email";
            return false;
        }

        private bool ValidatePassword(string password, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Enter password";
                return false;
            }


            if (password.Length < Password.MinLength)
            {
                errorMessage = "Password must contain at least 6 characters";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private bool ValidateConfirmPassword(string password, string confirmPassword, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                errorMessage = "Confirm password";
                return false;
            }

            if (password != confirmPassword)
            {
                errorMessage = "Differently password";
                return false;
            }

            errorMessage = null;
            return true;
        }

        #endregion

        #region Button Handlers

        private async void OnLoginClicked()
        {
            ClearAllErrors();

            string email = _loginEmailInputField.text.Trim();
            string password = _loginPasswordInputField.text;

            if (!ValidateEmail(email, out string emailError))
            {
                _errorLoginContainer.SetActive(true);
                _errorLoginText.text = emailError;
                return;
            }

            if (!ValidatePassword(password, out string passwordError))
            {
                _errorLoginContainer.SetActive(true);
                _errorLoginText.text = passwordError;
                return;
            }

            SetLoginInteractable(false);

            try
            {
                Result loginResult =
                    await _authService.SignInEmailAsync(Email.Create(email).Value, Password.Create(password).Value);
                if (loginResult.IsFailure)
                {
                    _errorLoginContainer.SetActive(true);
                    Debug.LogError(loginResult.Error);
                    _errorLoginText.text = loginResult.Error.Message;
                }
                else
                {
                    Debug.Log("Login successful");
                    //TODO: Sign in successful
                }
            }
            catch (Exception e)
            {
                _errorLoginContainer.SetActive(true);
                _errorLoginText.text = "Error connection";
                Debug.LogError(e);
            }
            finally
            {
                SetLoginInteractable(true);
            }
        }

        private async void OnRegisterClicked()
        {
            ClearAllErrors();

            string email = _registerEmailInputField.text.Trim();
            string password = _registerPasswordInputField.text;
            string confirmPassword = _registerConfirmPasswordInputField.text;
            string userName = email;

            if (!ValidateEmail(email, out string emailError))
            {
                _errorRegisterContainer.SetActive(true);
                _errorRegisterText.text = emailError;
                return;
            }

            if (!ValidatePassword(password, out string passwordError))
            {
                _errorRegisterContainer.SetActive(true);
                _errorRegisterText.text = passwordError;
                return;
            }

            if (!ValidateConfirmPassword(password, confirmPassword, out string confirmError))
            {
                _errorRegisterContainer.SetActive(true);
                _errorRegisterText.text = confirmError;
                return;
            }

            if (!ValidateAge(_registerAgeInputField.text, out int age, out string ageError))
            {
                _errorRegisterContainer.SetActive(true);
                _errorRegisterText.text = ageError;
                return;
            }

            SetRegisterInteractable(false);

            try
            {
                Result registerResult = await _authService.RegisterEmailAsync(Email.Create(email).Value,
                    UserName.Create(userName).Value,
                    Password.Create(password).Value,
                    age);

                if (registerResult.IsFailure)
                {
                    _errorRegisterContainer.SetActive(true);
                    _errorRegisterText.text = registerResult.Error.Message;
                }
                else
                {
                    Debug.Log("Registration successful");
                    //TODO: Registration complete
                }
            }
            catch (Exception e)
            {
                _errorRegisterContainer.SetActive(true);
                _errorRegisterText.text = "Error connection";
                Debug.LogError(e);
            }
            finally
            {
                SetRegisterInteractable(true);
            }
        }

        #endregion

        private void SetLoginInteractable(bool interactable)
        {
            _loginButton.SetAvailability(interactable);
        }

        private void SetRegisterInteractable(bool interactable)
        {
            _registerButton.SetAvailability(interactable);
        }

        private bool ValidateAge(string ageText, out int age, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(ageText) || !int.TryParse(ageText, out age))
            {
                errorMessage = "Enter a valid age";
                age = 0;
                return false;
            }

            if (age < 13 || age > 120)
            {
                errorMessage = "Incorrect age";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
