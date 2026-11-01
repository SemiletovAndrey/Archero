using System;
using Cysharp.Threading.Tasks;
using Persistence.Features.AuthProvider;
using Template.Scripts.DataHolders;
using Template.Utilities.Primitives;
using UnityEngine;

namespace Persistence.Features.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthProvider _authProvider;
        
        public event Action<UserData> LoggedIn;
        public event Action LogOutUser;

        public IModifierCollection<UserData> UserLoggedIn { get; }
        
        public AuthService(IAuthProvider authProvider)
        {
            _authProvider = authProvider;
        }

        public async UniTask InitAsync()
        {

        }

        public async UniTask<Result> SignInEmailAsync(Email email, Password password)
        {
            var component = _authProvider.GetRegisterComponent<EmailRegisterComponent, EmailRegisterComponentData>();
            Result<RegistrationUserModel> result = await component.SignInUser(email.Value, password.Value);

            if (result.IsFailure)
                return Result.Failure(result.Error);
            
            if (!_authProvider.TryGetUser(out var userId))
            {
                return Result.Failure(new Error("Auth.NoUser", "Не удалось получить идентификатор пользователя после входа"));
            }

            /*Result<bool> existsResult = await _userService.UserExistsOnServer(userId);
            if (existsResult.IsFailure)
                return Result.Failure(existsResult.Error);

            if (!existsResult.Value)
            {
                return Result.Failure(new Error("Auth.ProfileMissing",
                    "Profile is not found."));
            }*/

            UserData mock = new UserData();
            Error error = Error.None;
            Result<UserData> userResult = new Result<UserData>(mock,true, error);//await _userService.GetUser(userId, true);
            if (userResult.IsFailure)
                return Result.Failure(userResult.Error);

            LoggedIn?.Invoke(userResult.Value);
            return Result.Success();
        }

        public async UniTask<Result> RegisterEmailAsync(Email email, UserName userName, Password password, int age)
        {
            var component = _authProvider.GetRegisterComponent<EmailRegisterComponent, EmailRegisterComponentData>();
            Result<RegistrationUserModel> result = await component.RegisterNewUser(
                new EmailRegisterComponentData(email.Value, password.Value));

            if (result.IsFailure)
                return Result.Failure(result.Error);

            if (!_authProvider.TryGetUser(out var userId))
            {
                return Result.Failure(new Error("Auth", "Don't get user id after register"));
            }

            var userData = UserData.Builder
                .FromEmpty(userId)
                .WithEmail(email.Value)
                .WithUsername(userName.Value)
                .WithAge(age)
                .Bake();

            Result registerOnServerResult = null;//await _userService.RegisterNewUserAsync(userData);

            /*if (registerOnServerResult.IsFailure)
            {
                await TryRollbackFirebaseUser();
                return Result.Failure(registerOnServerResult.Error);
            }*/

            LoggedIn?.Invoke(userData);
            return Result.Success();
        }

        public async void LogOut()
        {
            bool tryLogout = await _authProvider.TryLogout();
            if (!tryLogout)
            {
                return;
            }

            LogOutUser?.Invoke();
            
            Debug.LogWarning("Logging out");
        }
        
        private async UniTask TryRollbackFirebaseUser()
        {
            try
            {
                await _authProvider.DeleteCurrentUserAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Rollback firebase user failed: {e}");
            }
        }
    }
}
