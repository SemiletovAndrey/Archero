using Gameplay.Features.EntityCore;
using Persistance.Features.EntityCore;
using Persistance.Features.GameLoopFeature;
using Template.Features.ClassFactory;
using Template.Features.GameFlow.Scripts;
using Template.Features.ProjectInit;
using UnityEngine;

namespace Gameplay.Scripts
{
    public class GameplaySceneInstaller : BaseInstaller
    {
        [SerializeField] private TestGameplay _testGameplay;
        public override void InstallBindings()
        {
            Container.Bind<IClassFactory>().To<ClassFactory>().AsSingle();

            base.InstallBindings();

            Container.BindInstance(new SceneFeatureSet(GetFeatures())).AsSingle();
            
            Container.Bind<TestGameplay>().FromInstance(_testGameplay).AsSingle();

            Container.BindInterfacesAndSelfTo<GameplaySceneEntryPoint>().AsSingle().NonLazy();
        }

        protected override void AddFeatures()
        {
            AddFeature<GameplayNavigationFeature>();
            AddFeature<EntityCoreFeature>();
            AddFeature<GameLoopFeature>();
            AddFeature<GameplayEntityCoreFeature>();
        }
    }
}