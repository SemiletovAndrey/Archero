using Template.Features.ProjectInit;

namespace Gameplay.Features.AI
{
    public class AiBrainFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IBrainsFactory, BrainsFactory>();
            BindAsSingle<IAiBrainContext, AiBrainContext>();
        }
    }
}