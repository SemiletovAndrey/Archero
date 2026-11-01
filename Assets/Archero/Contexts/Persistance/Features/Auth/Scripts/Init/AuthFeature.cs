using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Persistence.Features.Auth
{
    public class AuthFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IAuthService, AuthService>();
        }

        protected async override UniTask InitServices()
        {
            await ResolveAs<IAuthService, AuthService>().InitAsync();
        }
    }
}