using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Template.Features.ProjectInit
{
    public abstract class BaseInstaller : MonoInstaller, IFeatureFactory
    {
        [SerializeField] private List<BaseMonoBehaviourFeature> _monoBehaviourFeatures;

        private List<IBaseFeature> _features = new();
        private LinkedList<IInitializableFeature> _initializableFeatures;

        public override void InstallBindings()
        {
            Container.Bind<IFeatureFactory>().FromInstance(this).AsSingle();

            AddFeatures();
            BindFeatures();
        }

        protected abstract void AddFeatures();

        protected void AddFeature<T>() where T : IBaseFeature
        {
            _features.Add(CreateFeature<T>());
        }

        private void  BindFeatures()
        {
            foreach (var feature in _features)
            {
                feature.Install();
            }
        }

        public LinkedList<IInitializableFeature> GetFeatures()
        {
            if (_initializableFeatures != null)
            {
                return _initializableFeatures;
            }

            _initializableFeatures = new LinkedList<IInitializableFeature>();

            foreach (IInitializableFeature innerFeatures in _features.SelectMany(feature => feature.GetInnerFeatures()))
            {
                _initializableFeatures.AddLast(innerFeatures);
            }

            return _initializableFeatures;
        }

        public T CreateFeature<T>() where T : IBaseFeature
        {
            T feature;
            if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
            {
                feature = _monoBehaviourFeatures.OfType<T>().FirstOrDefault();

                if (feature is null)
                {
                    throw new Exception($"There is no monoBehaviour feature of type {typeof(T)}");
                }

                Container.Inject(feature);
            }
            else
            {
                feature = Container.Instantiate<T>();
            }

            return feature;
        }
    }
}