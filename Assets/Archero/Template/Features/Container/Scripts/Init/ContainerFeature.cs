using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.Container
{
    public class ContainerFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IContainerSystem, ContainerSystem>();
        }

        protected override UniTask InitSystems()
        {
            ResolveAs<IContainerSystem, ContainerSystem>().Init();
            return UniTask.CompletedTask;
        }
    }
}