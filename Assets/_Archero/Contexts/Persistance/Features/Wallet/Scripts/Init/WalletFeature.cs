using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Persistance.Features.Wallet
{
    public class WalletFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IWalletService, WalletService>();
        }
    }
}