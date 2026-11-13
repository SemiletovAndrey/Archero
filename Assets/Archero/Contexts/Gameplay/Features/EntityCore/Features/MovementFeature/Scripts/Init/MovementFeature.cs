using Gameplay.Features.EntityCore.Features.Scripts.Other.Recipes;
using Persistance.Features.EntityCore;
using Template.Features.ProjectInit;

namespace Gameplay.Features.MovementFeature
{
    public class MovementFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            Container.Bind<IEntityRecipe>().To<GhostEntityRecipe>().AsSingle();
            Container.Bind<IEntityRecipe>().To<HeroEntityRecipe>().AsSingle();
        }
    }
}