using Template.Features.ProjectInit;
using Template.Scripts.Init;

namespace DomainOfBattles.Contexts.Preloader
{
    public class ProjectInstaller : BaseInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ProjectInstaller>().FromInstance(this).WhenInjectedInto<ProjectBootstrapper>();

            base.InstallBindings();
        }

        protected override void AddFeatures()
        {
            AddFeature<TemplateFeatureGroup>();
        }
    }
}
