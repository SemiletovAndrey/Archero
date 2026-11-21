using Gameplay.Features.AI.MainHero;
using Gameplay.Features.Enemy;
using Template.Features.ProjectInit;

namespace Gameplay.Features.AI
{
    public class AiBrainFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IBrainsFactory, BrainsFactory>();
            BindAsSingle<IAiBrainContext, AiBrainContext>();
            
            Container.BindInterfacesAndSelfTo<HeroBrainBuilder>().AsSingle();
            Container.BindInterfacesAndSelfTo<GhostBrainBuilder>().AsSingle();
            Container.BindInterfacesAndSelfTo<TeleportetBrainBuilder>().AsSingle();
            
        }
    }
}