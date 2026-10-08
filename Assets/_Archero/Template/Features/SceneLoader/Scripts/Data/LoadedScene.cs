using Template.Features.ClassFactory;
using Template.Features.GameFlow.Scripts.Other.FeatureInitializer;
using UnityEngine.SceneManagement;
using Zenject;

namespace Template.Features.SceneLoader
{
    public class LoadedScene
    {
        public LoadedScene(Scene scene, DiContainer container, SceneFeatureSet features)
        {
            Scene = scene;
            Container = container;
            Features = features;
        }
 
        public Scene Scene { get; }
 
        /// <summary>Контейнер сцены (дочерний к ProjectContext).</summary>
        public DiContainer Container { get; }
 
        public SceneFeatureSet Features { get; }
 
        /// <summary>Фабрика именно этой сцены (см. SceneInstaller): создаёт объекты в контейнере сцены.</summary>
        public IClassFactory ClassFactory => Container.Resolve<IClassFactory>();
 
        public T Resolve<T>() => Container.Resolve<T>();

    }
}