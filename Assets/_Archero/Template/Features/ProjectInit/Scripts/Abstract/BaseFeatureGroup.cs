using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Template.Features.ProjectInit
{
    public abstract class BaseFeatureGroup : IBaseFeature
    {
        private IFeatureFactory _featureFactory;
        private List<IBaseFeature> _features = new();
        
        [Inject]
        private void Construct(DiContainer container, IFeatureFactory featureFactory)
        {
            Container = container;
            _featureFactory = featureFactory;
        }
        
        public virtual string FeatureName => GetType().Name;
        protected DiContainer Container { get; private set; }

        public LinkedList<IInitializableFeature> GetInnerFeatures()
        {
            LinkedList<IInitializableFeature> initializableFeatures = new();

            foreach (IInitializableFeature innerFeatures in _features.SelectMany(feature => feature.GetInnerFeatures()))
            {
                initializableFeatures.AddLast(innerFeatures);
            }
            
            return initializableFeatures;
        }

        protected abstract void AddFeatures();
        
        public virtual void Install()
        {
            AddFeatures();
            
            foreach (var feature in _features)
            {
                feature.Install();
            }
        }

        protected void AddFeature<T>() where T : IBaseFeature
        {
            T feature = _featureFactory.CreateFeature<T>();
            _features.Add(feature);
        }
    }
}