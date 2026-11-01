using Cysharp.Threading.Tasks;
using Template.Features.Firebase.Services;
using Template.Features.ProjectInit;

namespace Template.Features.Firebase.Init
{
    public class FirebaseFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IFirebaseService, FirebaseService>();
        }
        
        protected async override UniTask InitServices()
        {
            await ResolveAs<IFirebaseService, FirebaseService>().InitAsync();
        }
    }
}