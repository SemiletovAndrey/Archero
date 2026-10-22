using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Template.Features.ClassFactory;
using Template.Features.Config.Local;
using Template.Features.Firebase.Services;
using Template.Scripts.DataHolders;
using UnityEngine;

namespace Contexts.Persistence.Features.AuthProvider
{
    public class FirebaseAuthProvider : IAuthProvider
    {
        private readonly IClassFactory _classFactory;
        private readonly ILocalConfigService _localConfigService;
        private readonly IFirebaseService _firebaseService;

        private FirebaseAuthConfig _config;
        private List<BaseRegisterComponent> _registerComponents = new();

        private UniTaskCompletionSource _authStateChangedCompletionSource;

        public FirebaseAuthProvider(IClassFactory classFactory, ILocalConfigService localConfigService,
            IFirebaseService firebaseService)
        {
            _classFactory = classFactory;
            _localConfigService = localConfigService;
            _firebaseService = firebaseService;
        }

        public FirebaseAuth Auth { get; private set; }

        public bool IsGuest
        {
            get
            {
                FirebaseUser user = Auth.CurrentUser;
                if (user == null)
                {
                    return true;
                }

                bool hasProviders =
                    user.ProviderData?.Any(p => p.ProviderId != AuthProviderConstants.Providers.Firebase) == true;
                bool hasEmail = !string.IsNullOrEmpty(user.Email);

                return !hasProviders && !hasEmail;
            }
        }

        public bool IsFirebaseInitialized { get; private set; }

        public async UniTask InitAsync()
        {
            _config = _localConfigService.GetConfig<FirebaseAuthConfig>();
            Auth = _firebaseService.Auth;
            Auth.StateChanged += OnAuthStateChanged;
            AddComponents();
        }

        public FirebaseAuthConfig GetConfig()
        {
            return _config;
        }

        public TComponent GetRegisterComponent<TComponent, TData>()
            where TComponent : BaseRegisterComponentT<TData>
            where TData : RegisterComponentData
        {
            return _registerComponents.OfType<TComponent>().First();
        }

        public bool TryGetUser(out string userId)
        {
            FirebaseUser user = Auth.CurrentUser;

            if (user == null)
            {
                userId = null;
                return false;
            }

            userId = user.UserId;
            return true;
        }

        public async UniTask<bool> WaitForUserAsync()
        {
            if (TryGetUser(out _))
            {
                return true;
            }

            _authStateChangedCompletionSource ??= new UniTaskCompletionSource();

            try
            {
                await _authStateChangedCompletionSource.Task
                    .Timeout(TimeSpan.FromSeconds(3));
            }
            catch (TimeoutException)
            {
                return TryGetUser(out _);
            }
            finally
            {
                _authStateChangedCompletionSource = null;
            }

            return TryGetUser(out _);
        }

        public async UniTask<bool> TryLogout()
        {
            FirebaseUser user = Auth.CurrentUser;

            if (user == null)
            {
                return false;
            }
    
            try
            {
                Debug.Log($"Logging out user with provider: {user.ProviderId}");
                
                foreach (var providerData in user.ProviderData)
                {
                    try
                    {
                        switch (providerData.ProviderId)
                        {
                            /*case AuthProviderConstants.Providers.Google:
                                Debug.Log("Signing out from Google");
                                await GoogleSignIn.DefaultInstance.SignIn();
                                GoogleSignIn.DefaultInstance.SignOut();
                                break;*/
                    
                            case AuthProviderConstants.Providers.Apple:
                                Debug.Log("Signing out from Apple");

                                break;
                    
                            default:
                                Debug.Log($"Provider {providerData.ProviderId} doesn't require explicit sign out");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to sign out from {providerData.ProviderId}: {ex.Message}");
                    }
                }
                
                Auth.SignOut();
                
                Debug.Log("✅ Logout successful");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public async UniTask<Result> DeleteCurrentUserAsync()
        {
            FirebaseUser user = Auth.CurrentUser;

            if (user == null)
            {
                return Result.Failure(new Error("Auth.NoUser", "User is not authenticated"));
            }

            try
            {
                await user.DeleteAsync();
                Debug.Log("✅ Firebase user deleted (rollback)");
                return Result.Success();
            }
            catch (FirebaseException ex)
            {
                string errorMessage = ex.ErrorCode switch
                {
                    (int)AuthError.RequiresRecentLogin => "Please re-login and try again",
                    (int)AuthError.UserNotFound => "User not found",
                    (int)AuthError.NetworkRequestFailed => "Network error. Please check your connection",
                    _ => $"Failed to delete user: {ex.Message}"
                };

                Debug.LogError($"❌ Firebase user deletion failed: {errorMessage}");
                return Result.Failure(new Error("Auth.DeleteFailed", errorMessage));
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Unexpected error during user deletion: {ex.Message}");
                return Result.Failure(new Error("Auth.DeleteFailed", "Unexpected error during user deletion"));
            }
        }

        public AuthProviderType GetAuthProvider(FirebaseUser user)
        {
            if (user == null)
                return AuthProviderType.None;

            foreach (var provider in user.ProviderData)
            {
                switch (provider.ProviderId)
                {
                    case "google.com":
                        return AuthProviderType.Google;

                    case "apple.com":
                        return AuthProviderType.Apple;

                    default:
                        return AuthProviderType.None;
                }
            }

            return AuthProviderType.None;
        }

        public async UniTask<Result> ReauthenticateUser()
        {
            FirebaseUser user = Auth.CurrentUser;

            if (user == null)
            {
                return Result.Failure(new Error("Auth.NoUser", "User is not authenticated"));
            }

            try
            {
                var provider =
                    user.ProviderData.FirstOrDefault(p => p.ProviderId != AuthProviderConstants.Providers.Firebase);

                if (provider == null)
                {
                    return Result.Failure(new Error("Auth.NoProvider",
                        "No authentication provider found for reauthentication"));
                }

                Debug.Log($"Reauthenticating with provider: {provider.ProviderId}");

                switch (provider.ProviderId)
                {
                    case AuthProviderConstants.Providers.Google:
                        return await ReauthenticateWithGoogle();

                    case AuthProviderConstants.Providers.Apple:
                        return await ReauthenticateWithApple();

                    case "password":
                        return Result.Failure(new Error("Auth.EmailReauthRequired",
                            "Please re-enter your password to delete account"));

                    default:
                        return Result.Failure(new Error("Auth.UnsupportedProvider",
                            $"Reauthentication not supported for provider: {provider.ProviderId}"));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Reauthentication failed: {ex.Message}");
                return Result.Failure(new Error("Auth.ReauthFailed", ex.Message));
            }
        }
        
        private async UniTask<Result> ReauthenticateWithGoogle()
        {
            /*try
            {
                GoogleRegisterComponent googleComponent =
                    GetRegisterComponent<GoogleRegisterComponent, GoogleRegisterComponentData>();

                var googleResult = await googleComponent.Register(new GoogleRegisterComponentData());

                if (googleResult.IsSuccess)
                {
                    Debug.Log("✅ Google reauthentication successful");
                    return Result.Success();
                }

                Debug.LogError($"❌ Google reauthentication failed: {googleResult.Error.Message}");
                return Result.Failure(googleResult.Error);
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Google reauthentication exception: {ex.Message}");
                return Result.Failure(new Error("Auth.GoogleReauthFailed", ex.Message));
            }*/
            return Result.Failure(new Error(" ","Google authentication failed"));
        }

        private async UniTask<Result> ReauthenticateWithApple()
        {
            /*try
            {
                AppleRegisterComponent appleComponent =
                    GetRegisterComponent<AppleRegisterComponent, AppleRegisterComponentData>();

                var appleResult = await appleComponent.Register(new AppleRegisterComponentData());

                if (appleResult.IsSuccess)
                {
                    Debug.Log("✅ Apple reauthentication successful");
                    return Result.Success();
                }

                Debug.LogError($"❌ Apple reauthentication failed: {appleResult.Error.Message}");
                return Result.Failure(appleResult.Error);
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Apple reauthentication exception: {ex.Message}");
                return Result.Failure(new Error("Auth.AppleReauthFailed", ex.Message));
            }*/
            return Result.Failure(new Error(" ","IOS authentication failed"));
        }

        private void AddComponents()
        {
            AddComponent<EmailRegisterComponent>();
        }

        private void OnAuthStateChanged(object sender, EventArgs e)
        {
            if (Auth.CurrentUser == null)
            {
                return;
            }

            _authStateChangedCompletionSource?.TrySetResult();
        }

        private void AddComponent<TComponent>() where TComponent : BaseRegisterComponent
        {
            TComponent component = _classFactory.Create<TComponent>();
            component.Init();

            _registerComponents.Add(component);
        }
    }
}
