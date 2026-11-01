using Template.Features.DataStorage;
using Template.Features.ProjectInit;

namespace Persistance.Features.User.Features.JsonUser
{
    public class JsonUserFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IDataProvider<PlayerData>, PlayerDataProvider>();
        }
    }
}