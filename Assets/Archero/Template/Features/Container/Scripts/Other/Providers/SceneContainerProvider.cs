using System.Collections.Generic;
using Template.Features.ClassFactory;
using Template.Features.ProjectInit;
using Template.Scripts.Utils;
using UnityEngine;
using Zenject;

namespace Template.Features.Container
{
    public class SceneContainerProvider : BaseInitializableBehaviour, IContainerProvider
    {
        [SerializeField] private List<BaseMonoBehaviourContainer> _monoBehaviourContainers;
        
        private IClassFactory _classFactory;
        private IContainerSystem _containerSystem;
        
        private List<IContainer> _containers = new();

        [Inject]
        private void Construct(IClassFactory classFactory, IContainerSystem containerSystem)
        {
            _classFactory = classFactory;
            _containerSystem = containerSystem;
        }

        public override void Init()
        {
            AddContainers();
            _containerSystem.AddProvider(this);
        }

        private void OnDestroy()
        {
            _containerSystem.RemoveProvider(this);
        }

        public virtual bool TryGet<TContainer>(out TContainer container) where TContainer : IContainer
        {
            container = _monoBehaviourContainers.FirstOrDefaultOfType<TContainer>()
                        ?? _containers.FirstOrDefaultOfType<TContainer>();

            return container != null;
        }

        protected virtual void AddContainers()
        {
            
        }
        
        protected void AddContainer<TContainer>() where TContainer : IContainer
        {
            TContainer container = _classFactory.Create<TContainer>();
            _containers.Add(container);
        }
    }
}