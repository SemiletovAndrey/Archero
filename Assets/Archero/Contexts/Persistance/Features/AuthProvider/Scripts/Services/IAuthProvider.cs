using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Template.Scripts.DataHolders;

namespace Persistence.Features.AuthProvider
{
    public interface IAuthProvider
    {
        public bool IsFirebaseInitialized { get; }
        
        public UniTask InitAsync();
        
        public FirebaseAuthConfig GetConfig();

        public TComponent GetRegisterComponent<TComponent, TData>()
            where TComponent : BaseRegisterComponentT<TData>
            where TData : RegisterComponentData;

        public bool TryGetUser(out string userId);
        public UniTask<bool> WaitForUserAsync();

        public UniTask<bool> TryLogout();
        public bool IsGuest { get; }

        public AuthProviderType GetAuthProvider(FirebaseUser user);
        public UniTask<Result> DeleteCurrentUserAsync();
        public UniTask<Result> ReauthenticateUser();
    }
}
