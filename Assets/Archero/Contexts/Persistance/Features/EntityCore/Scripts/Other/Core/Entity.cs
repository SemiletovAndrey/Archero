using System;
using System.Collections.Generic;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public sealed class Entity
    {
        public int Id { get; }
        public EntityType Type { get; }
        public EntityState State { get; internal set; }
        public bool IsAlive => State == EntityState.Active;

        internal int WorldIndex = -1;

        private readonly EntityWorld _world;
        private readonly List<IEntitySystem> _systems = new(4);
        private object[] _components;

        internal Entity(int id, EntityType type, EntityWorld world)
        {
            Id = id;
            Type = type;
            _world = world;
            _components = new object[Math.Max(8, ComponentRegistry.Count)];
        }
        
        public Entity Add<T>(T component) where T : class
        {
            return AddInternal(ComponentId<T>.Value, typeof(T), component);
        }

        public Entity Add(Type type, object component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (!type.IsInstanceOfType(component))
                throw new ArgumentException($"{component.GetType()} is not {type}");

            return AddInternal(ComponentRegistry.GetId(type), type, component);
        }

        public bool Has<T>() where T : class
        {
            int id = ComponentId<T>.Value;
            return (uint)id < (uint)_components.Length && _components[id] != null;
        }

        public bool TryGet<T>(out T component) where T : class
        {
            int id = ComponentId<T>.Value;

            if ((uint)id < (uint)_components.Length && _components[id] is T found)
            {
                component = found;
                return true;
            }

            component = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            if (!TryGet(out T component))
                throw new InvalidOperationException($"Entity #{Id} ({Type}) has no component {typeof(T).Name}");

            return component;
        }

        public bool Remove<T>() where T : class
        {
            int id = ComponentId<T>.Value;

            if ((uint)id >= (uint)_components.Length || _components[id] == null)
                return false;

            _components[id] = null;
            return true;
        }

        private Entity AddInternal(int id, Type type, object component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component), $"Null {type.Name} for entity #{Id}");

            if (State == EntityState.Disposed)
                throw new InvalidOperationException($"Entity #{Id} is disposed");

            if (id >= _components.Length)
                Array.Resize(ref _components, Math.Max(id + 1, _components.Length * 2));

            if (_components[id] != null)
                throw new ArgumentException($"Entity #{Id} ({Type}) already has {type.Name}");

            _components[id] = component;
            return this;
        }

        public Entity AddSystem(IEntitySystem system)
        {
            if (system == null)
                throw new ArgumentNullException(nameof(system));

            if (State >= EntityState.Releasing)
                throw new InvalidOperationException($"Entity #{Id} is releasing/disposed");

            if (_systems.Contains(system))
                throw new ArgumentException($"{system.GetType().Name} is already registered");

            _systems.Add(system);
            
            if (State == EntityState.Active)
            {
                InitSystem(system);
                _world.Schedule(this, system);
            }

            return this;
        }

        public void Release() => _world.Release(this);


        internal void Activate()
        {
            if (State != EntityState.Created)
                throw new InvalidOperationException($"Entity #{Id} already activated");

            State = EntityState.Initializing;

            for (int i = 0; i < _systems.Count; i++)
                InitSystem(_systems[i]);

            State = EntityState.Active;

            for (int i = 0; i < _systems.Count; i++)
                _world.Schedule(this, _systems[i]);
        }

        internal void Deactivate()
        {
            if (State == EntityState.Disposed)
                return;

            bool initialized = State != EntityState.Created;

            if (initialized)
            {
                for (int i = _systems.Count - 1; i >= 0; i--)
                {
                    if (_systems[i] is IDisposableSystem disposable)
                        Safe(disposable);
                }
            }

            for (int i = 0; i < _components.Length; i++)
            {
                if (_components[i] is IDisposable disposable)
                    Safe(disposable);

                _components[i] = null;
            }

            _systems.Clear();
            State = EntityState.Disposed;
        }

        private void InitSystem(IEntitySystem system)
        {
            if (system is IInitializableSystem init)
                init.OnInit(this);
        }

        private static void Safe(IDisposableSystem system)
        {
            try { system.OnDispose(); }
            catch (Exception e) { Debug.LogException(e); }
        }

        private static void Safe(IDisposable disposable)
        {
            try { disposable.Dispose(); }
            catch (Exception e) { Debug.LogException(e); }
        }

        public override string ToString() => $"Entity #{Id} ({Type}, {State})";
    }
}
