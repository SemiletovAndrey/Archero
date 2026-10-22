using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Contexts.Persistence.Features.AuthProvider
{
    public class AuthProviderFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IAuthProvider, FirebaseAuthProvider>();
        }

        protected override async UniTask InitServices()
        {
            await ResolveAs<IAuthProvider, FirebaseAuthProvider>().InitAsync();
        }
    }
}