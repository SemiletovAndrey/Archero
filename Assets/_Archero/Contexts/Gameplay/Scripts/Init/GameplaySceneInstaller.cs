using Template.Features.ClassFactory;
using Template.Features.GameFlow.Scripts;
using Template.Features.ProjectInit;

namespace Gameplay.Scripts
{
    public class GameplaySceneInstaller : BaseInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IClassFactory>().To<ClassFactory>().AsSingle();

            base.InstallBindings();

            Container.BindInstance(new SceneFeatureSet(GetFeatures())).AsSingle();

            Container.BindInterfacesAndSelfTo<GameplaySceneEntryPoint>().AsSingle().NonLazy();
        }

        protected override void AddFeatures()
        {
            
        }
    }
}