using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.NewInputFeature
{
    public class NewInputFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IInputBus, InputBus>();
            BindAsSingle<IInputProviderManager, InputProviderManager>();
        }
    }
}