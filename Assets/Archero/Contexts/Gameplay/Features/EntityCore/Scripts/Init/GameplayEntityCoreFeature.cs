using Gameplay.Features.EntityCore.Features.Scripts.Other.Recipes;
using Template.Features.ProjectInit;

namespace Gameplay.Features.EntityCore
{
    public class GameplayEntityCoreFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IEntityConfigService, IEntityConfigService>();
            
            Container.BindInterfacesTo<GhostEntityRecipe>().AsSingle();
            Container.BindInterfacesTo<HeroEntityRecipe>().AsSingle();
            Container.BindInterfacesTo<ProjectileEntityRecipe>().AsSingle();
            Container.BindInterfacesTo<TeleportatorEntityRecipe>().AsSingle();
        }
    }
}
