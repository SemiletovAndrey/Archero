using Template.Features.ProjectInit;
using Zenject;

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