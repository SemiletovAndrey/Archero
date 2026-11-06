using Template.Features.ProjectInit;

namespace Persistance.Features.GameLoopFeature
{
    public class GameLoopFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameplayLoopService>().AsSingle();
        }
    }
}