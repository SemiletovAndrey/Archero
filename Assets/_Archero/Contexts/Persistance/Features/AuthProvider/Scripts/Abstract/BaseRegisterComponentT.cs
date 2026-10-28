using Cysharp.Threading.Tasks;
using Template.Scripts.DataHolders;
using Zenject;

namespace Persistence.Features.AuthProvider
{
    public abstract class BaseRegisterComponentT<TData> : BaseRegisterComponent
        where TData : RegisterComponentData
    {
        [Inject]
        private void Construct(IAuthProvider authProvider)
        {
            AuthProvider = authProvider as FirebaseAuthProvider;
        }
        
        protected FirebaseAuthProvider AuthProvider { get; private set; }
     
        public abstract UniTask<Result<RegistrationUserModel>> RegisterNewUser(TData data);
        public abstract UniTask<Result<RegistrationUserModel>> SignInUser(string email, string password);
    }
}