using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Persistence.Features.User.Scripts
{
    public class UserFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IUserRepository, UserRepository>();
            BindAsSingle<IUserService, UserService>();
        }

        protected async override UniTask InitServices()
        {
            await ResolveAs<IUserService, UserService>().InitAsync();
        }
        
        
    }
}
