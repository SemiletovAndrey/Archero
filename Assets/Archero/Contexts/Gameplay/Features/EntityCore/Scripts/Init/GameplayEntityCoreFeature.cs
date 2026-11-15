using Gameplay.Features.EntityCore.Features.Scripts.Other.Recipes;
using Template.Features.ProjectInit;

namespace Gameplay.Features.EntityCore
{
    public class GameplayEntityCoreFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            Container.BindInterfacesTo<GhostEntityRecipe>().AsSingle();
            Container.BindInterfacesTo<HeroEntityRecipe>().AsSingle();
            Container.BindInterfacesTo<ProjectileEntityRecipe>().AsSingle();
        }
    }
}