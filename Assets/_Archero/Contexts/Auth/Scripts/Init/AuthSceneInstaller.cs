using Auth.Scripts.Navigation;
using Template.Features.ClassFactory;
using Template.Features.GameFlow.Scripts;
using Template.Features.ProjectInit;

namespace Auth.Scripts.Init
{
    public class AuthSceneInstaller : BaseInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IClassFactory>().To<ClassFactory>().AsSingle();
            
            base.InstallBindings();
            
            Container.BindInstance(new SceneFeatureSet(GetFeatures())).AsSingle();
            
            Container.BindInterfacesAndSelfTo<AuthSceneEntryPoint>().AsSingle().NonLazy();
        }

        protected override void AddFeatures()
        {
            AddFeature<AuthNavigationFeature>();
        }
    }
}