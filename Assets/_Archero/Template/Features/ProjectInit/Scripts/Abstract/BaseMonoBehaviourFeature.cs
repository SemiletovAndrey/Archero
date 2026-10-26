using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using UnityEngine;
using Zenject;

namespace Template.Features.ProjectInit
{
    public abstract class BaseMonoBehaviourFeature : MonoBehaviour, IInitializableFeature
    {
        private IClassFactory _classFactory;

        protected DiContainer Container { get; private set; }
        public virtual string FeatureName => GetType().Name;

        protected abstract void InstallBindings();
        
        public LinkedList<IInitializableFeature> GetInnerFeatures()
        {
            LinkedList<IInitializableFeature> features = new();
            features.AddLast(this);
            
            return features;
        }
        
        public void SetContainer(DiContainer container)
        {
            Container = container;
        }

        public void Install()
        {
            InstallBindings();
        }

        public async UniTask InitFeature()
        {
            AddPopups();
            await InitServices();
            await InitSystems();
            await AddTrackers();
        }

        protected virtual UniTask InitServices()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask InitSystems()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask AddTrackers()
        {
            return UniTask.CompletedTask;
        }

        protected virtual void AddPopups()
        {
            
        }

        protected ConcreteIdArgConditionCopyNonLazyBinder BindAsSingle<TParent, TChild>() where TChild : TParent
        {
            return Container.BindInterfacesTo<TChild>().AsSingle();
        }
        
        protected ConcreteIdArgConditionCopyNonLazyBinder BindAsCashed<TParent, TChild>() where TChild : TParent
        {
            return Container.BindInterfacesTo<TChild>().AsCached();
        }
        
        protected TChild ResolveAs<TParent, TChild>() where TChild : class, TParent
        {
            return Container.Resolve<TParent>() as TChild;
        }
        
        protected T CreateInstance<T>()
        {
            _classFactory ??= Container.Resolve<IClassFactory>();
            return _classFactory.Create<T>();
        }
        
        protected T CreateInstanceAndBind<T>()
        {
            T instance = CreateInstance<T>();
            Container.BindInterfacesTo<T>().FromInstance(instance).AsCached();
            
            return instance;
        }
    }
}