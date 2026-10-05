using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Template.Features.Cheat;
using Template.Features.ClassFactory;
using Template.Features.Log;
using UnityEngine;
using Zenject;

namespace Template.Features.ProjectInit
{
    public abstract class BaseMonoBehaviourFeature : MonoBehaviour, IInitializableFeature
    {
        private IClassFactory _classFactory;
        private IDebugLogger _logger;
        
        [Inject]
        private void Construct(DiContainer container)
        {
            Container = container;
        }

        protected DiContainer Container { get; private set; }
        public virtual string FeatureName => GetType().Name;

        protected abstract void InstallBindings();
        
        public LinkedList<IInitializableFeature> GetInnerFeatures()
        {
            LinkedList<IInitializableFeature> features = new();
            features.AddLast(this);
            
            return features;
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
            
            AddCheats();
        }

        protected virtual UniTask InitServices()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask InitSystems()
        {
            return UniTask.CompletedTask;
        }
        
        protected virtual void AddCheats()
        {
        }

        protected virtual UniTask AddTrackers()
        {
            return UniTask.CompletedTask;
        }

        protected virtual void AddPopups()
        {
            
        }

        protected virtual IDebugLogger CreateLogger()
        {
            return LoggerFactory.CreateFeatureLogger(FeatureName);
        }

        protected ConcreteIdArgConditionCopyNonLazyBinder BindAsSingle<TParent, TChild>() where TChild : TParent
        {
            AddLoggerFor<TChild>();
            return Container.BindInterfacesTo<TChild>().AsSingle();
        }
        
        protected ConcreteIdArgConditionCopyNonLazyBinder BindAsCashed<TParent, TChild>() where TChild : TParent
        {
            AddLoggerFor<TChild>();
            return Container.BindInterfacesTo<TChild>().AsCached();
        }
        
        private void AddLoggerFor<TTarget>()
        {
            _logger ??= CreateLogger();
            Container.Bind<IDebugLogger>().FromInstance(_logger).AsCached().WhenInjectedInto<TTarget>();
        }
        
        protected TChild ResolveAs<TParent, TChild>() where TChild : class, TParent
        {
            return Container.Resolve<TParent>() as TChild;
        }
        
        protected TCheat CreateAndInitCheat<TCheat>() where TCheat : BaseCheatHandler
        {
            TCheat cheat = CreateInstance<TCheat>();
            cheat.Init();
            
            Container.BindInterfacesTo<TCheat>().FromInstance(cheat).AsSingle();
            
            return cheat;
        }
        
        protected T CreateInstance<T>()
        {
            AddLoggerFor<T>();
            
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