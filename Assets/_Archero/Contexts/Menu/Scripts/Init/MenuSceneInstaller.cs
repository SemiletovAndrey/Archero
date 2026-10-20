using Menu.Scripts.Navigation;
using Template.Features.ClassFactory;
using Template.Features.GameFlow.Scripts;
using Template.Features.ProjectInit;

namespace Menu.Scripts.Init
{
    public class MenuSceneInstaller : BaseInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IClassFactory>().To<ClassFactory>().AsSingle();
            
            base.InstallBindings();
            
            Container.BindInstance(new SceneFeatureSet(GetFeatures())).AsSingle();
            
            Container.BindInterfacesAndSelfTo<MenuSceneEntryPoint>().AsSingle().NonLazy();
        }

        protected override void AddFeatures()
        {
            AddFeature<MenuNavigationFeature>();
        }
    }
}