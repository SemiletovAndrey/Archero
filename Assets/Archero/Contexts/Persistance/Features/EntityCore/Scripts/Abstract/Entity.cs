using System;
using System.Collections.Generic;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public partial class Entity : IDisposable
    {
        private readonly Dictionary<Type, IEntityComponent> _components = new();

        private readonly List<IEntitySystem> _entitySystems = new();

        private readonly List<IInitializableSystem> _initializableSystems = new();
        private readonly List<IUpdatableSystem> _updatableSystems = new();
        private readonly List<IDisposableSystem> _disposableSystems = new();

        private bool _isInit;

        public void Init()
        {
            foreach (IInitializableSystem init in _initializableSystems)
            {
                init.OnInit(this);
            }

            _isInit = true;
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_isInit)
                return;

            foreach (IUpdatableSystem updatableSystem in _updatableSystems)
            {
                updatableSystem.OnUpdate(deltaTime);
            }
        }

        public void Dispose()
        {
            foreach (IDisposableSystem disposableSystem in _disposableSystems)
            {
                disposableSystem.OnDispose();
            }

            _isInit = false;
        }

        public Entity AddComponent<TComponent>(TComponent component) where TComponent : class, IEntityComponent
        {
            _components.Add(typeof(TComponent), component);
            return this;
        }

        public bool HasComponent<TComponent>() where TComponent : class, IEntityComponent
        {
            return _components.ContainsKey(typeof(TComponent));
        }

        public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : class, IEntityComponent
        {
            if (_components.TryGetValue(typeof(TComponent), out IEntityComponent entityComponent))
            {
                component = (TComponent)entityComponent;
                return true;
            }

            component = null;
            return false;
        }

        public TComponent GetComponent<TComponent>() where TComponent : class, IEntityComponent
        {
            if (!TryGetComponent(out TComponent component))
                throw new ArgumentException($"Entity not exist {typeof(TComponent)}");

            return component;
        }

        public Entity AddSystem(IEntitySystem system)
        {
            if (_entitySystems.Contains(system))
                throw new ArgumentException($"{system.GetType()}  is already registered");

            _entitySystems.Add(system);

            if (system is IInitializableSystem initializableSystem)
            {
                _initializableSystems.Add(initializableSystem);

                if (_isInit)
                    initializableSystem.OnInit(this);
            }

            if (system is IUpdatableSystem updatableSystem)
                _updatableSystems.Add(updatableSystem);

            if (system is IDisposableSystem disposableSystem)
                _disposableSystems.Add(disposableSystem);

            return this;
        }
    }
}