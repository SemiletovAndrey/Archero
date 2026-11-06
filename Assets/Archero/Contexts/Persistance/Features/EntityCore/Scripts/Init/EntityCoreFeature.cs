using Template.Features.ProjectInit;

namespace Persistance.Features.EntityCore
{
    public class EntityCoreFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EntityLifeService>().AsSingle();
            BindAsSingle<IEntitiesFactory, EntitiesFactory>();
        }
    }
}