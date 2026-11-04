using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Persistance.Features.Wallet
{
    public class WalletFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IWalletService, WalletService>().NonLazy();
        }

        protected override UniTask InitServices()
        {
            ResolveAs<IWalletService, WalletService>().Init();
            return base.InitServices();
        }
    }
}