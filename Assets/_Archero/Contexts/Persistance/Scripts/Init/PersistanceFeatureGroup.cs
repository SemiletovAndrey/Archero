using Contexts.Persistence.Features.AuthProvider;
using Persistance.Features.User.Features.JsonUser;
using Persistance.Features.Wallet;
using Persistence.Features.Auth;
using Template.Features.ProjectInit;

namespace _Archero.Contexts.Persistance.Scripts.Init
{
    public class PersistanceFeatureGroup : BaseFeatureGroup
    {
        protected override void AddFeatures()
        {
            AddFeature<AuthFeature>();
            AddFeature<AuthProviderFeature>();
            AddFeature<WalletFeature>();
            AddFeature<JsonUserFeature>();
        }
    }
}