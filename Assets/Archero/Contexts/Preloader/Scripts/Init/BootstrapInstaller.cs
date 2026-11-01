using Template.Features.ProjectInit;

namespace DomainOfBattles.Contexts.Preloader
{
    public class BootstrapInstaller : BaseInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BootstrapEntryPoint>().AsSingle().NonLazy();
        }

        protected override void AddFeatures()
        {
            
        }
    }
}