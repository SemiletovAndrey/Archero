using Template.Features.ClassFactory;
using Template.Features.GameFlow.Scripts;
using Template.Features.GameStateMachine;
using Template.Features.ProjectInit;
using Template.Scripts.Init;

namespace DomainOfBattles.Contexts.Preloader
{
    public class ProjectInstaller : BaseInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IClassFactory>().To<ClassFactory>().AsSingle();
            Container.Bind<IGameStateMachine>().To<GameStateMachine>().AsSingle();
            Container.Bind<IFeatureInitializer>().To<FeatureInitializer>().AsSingle();
            
            base.InstallBindings();
            Container.BindInstance(new GlobalFeatureSet(GetFeatures()));
        }

        protected override void AddFeatures()
        {
            AddFeature<TemplateFeatureGroup>();
        }
    }
}
