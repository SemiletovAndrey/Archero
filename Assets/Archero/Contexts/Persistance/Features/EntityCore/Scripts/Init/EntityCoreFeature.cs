using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Persistance.Features.EntityCore
{
    public class EntityCoreFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EntityLifeService>().AsSingle();
            BindAsSingle<IEntitiesFactory, EntitiesFactory>();
            BindAsSingle<IMonoEntityFactory, MonoEntityFactory>();
        }

        protected override async UniTask InitSystems()
        {
            await ResolveAs<IMonoEntityFactory, MonoEntityFactory>().Init();
            await base.InitSystems();
        }
    }
}