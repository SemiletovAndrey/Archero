using Template.Features.ProjectInit;
using Zenject;

namespace Template.Features.ClassFactory
{
    public class SceneContextProvider : BaseInitializableBehaviour
    {
        private IClassFactory _classFactory;
        private DiContainer _container;
        
        [Inject]
        private void Construct(IClassFactory classFactory, DiContainer diContainer)
        {
            _classFactory = classFactory;
            _container = diContainer;
        }
        
        public override void Init()
        {
            _classFactory.SetContainer(_container);
        }
    }
}