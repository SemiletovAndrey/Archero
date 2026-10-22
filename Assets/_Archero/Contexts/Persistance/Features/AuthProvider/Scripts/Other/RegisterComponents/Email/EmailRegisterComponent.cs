using Contexts.Persistence.Features.Auth;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Template.Scripts.DataHolders;
using UnityEngine;

namespace Contexts.Persistence.Features.AuthProvider
{
    public class EmailRegisterComponent : BaseRegisterComponentT<EmailRegisterComponentData>
    {
        private FirebaseAuth _auth;

        public override void Init()
        {
            _auth = AuthProvider.Auth;

            if (_auth == null)
            {
                Debug.LogError("FirebaseAuth initialization failed!");
            }
        }

        public override async UniTask<Result<RegistrationUserModel>> RegisterNewUser(EmailRegisterComponentData data)
        {
            if (_auth == null)
                return Result.Failure<RegistrationUserModel>(new Error("Firebase.AuthNull",
                    "FirebaseAuth is not init."));

            try
            {
                AuthResult result = await _auth.CreateUserWithEmailAndPasswordAsync(data.Email, data.Password);
                FirebaseUser user = result.User;
                return new RegistrationUserModel(user.UserId, data.Email, RegistrationType.Email);
            }
            catch (FirebaseException e) when (e.ErrorCode == (int)AuthError.EmailAlreadyInUse)
            {
                return Result.Failure<RegistrationUserModel>(new Error("EmailAlreadyInUse",
                    "User already exists."));
            }
            catch (FirebaseException e)
            {
                string errorMessage = ((AuthError)e.ErrorCode).ToString();
                return Result.Failure<RegistrationUserModel>(new Error(errorMessage, errorMessage));
            }
        }

        public override async UniTask<Result<RegistrationUserModel>> SignInUser(string email, string password)
        {
            if (_auth == null)
                return Result.Failure<RegistrationUserModel>(new Error("Firebase.AuthNull",
                    "FirebaseAuth not init."));

            try
            {
                AuthResult result = await _auth.SignInWithEmailAndPasswordAsync(email, password);
                return new RegistrationUserModel(result.User.UserId, email, RegistrationType.Email);
            }
            catch (FirebaseException e) when (e.ErrorCode == (int)AuthError.WrongPassword)
            {
                return Result.Failure<RegistrationUserModel>(new Error("WrongPassword", "Wrong password."));
            }
            catch (FirebaseException e) when (e.ErrorCode == (int)AuthError.UserNotFound)
            {
                return Result.Failure<RegistrationUserModel>(new Error("UserNotFound",
                    "User not found with this email."));
            }
            catch (FirebaseException e)
            {
                string errorMessage = ((AuthError)e.ErrorCode).ToString();
                return Result.Failure<RegistrationUserModel>(new Error(errorMessage, errorMessage));
            }
        }
    }
}