using System;
using System.Collections.Generic;

namespace Persistance.Features.EntityCore
{
    public sealed class EntityWorld : IEntityWorld, IDisposable
    {
        public event Action<Entity> Added;
        public event Action<Entity> Released;

        private readonly List<Entity> _entities = new();
        private readonly List<Entity> _releaseQueue = new();
        private readonly List<Slot<IUpdatableSystem>> _update = new();
        private readonly List<Slot<IFixedUpdatableSystem>> _fixedUpdate = new();

        private int _nextId;
        private bool _disposed;

        public int Count => _entities.Count;

        public Entity Create(EntityType type)
        {
            ThrowIfDisposed();
            return new Entity(++_nextId, type, this);
        }

        public void Add(Entity entity)
        {
            ThrowIfDisposed();

            entity.Activate();
            entity.WorldIndex = _entities.Count;
            _entities.Add(entity);

            Added?.Invoke(entity);
        }

        public void Release(Entity entity)
        {
            if (entity.State != EntityState.Active)
                return;

            entity.State = EntityState.Releasing;
            _releaseQueue.Add(entity);
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _update.Count; i++)
            {
                Slot<IUpdatableSystem> slot = _update[i];

                if (slot.Owner.State == EntityState.Active)
                    slot.System.OnUpdate(deltaTime);
            }

            FlushReleases();
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            for (int i = 0; i < _fixedUpdate.Count; i++)
            {
                Slot<IFixedUpdatableSystem> slot = _fixedUpdate[i];

                if (slot.Owner.State == EntityState.Active)
                    slot.System.OnFixedUpdate(fixedDeltaTime);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            for (int i = _entities.Count - 1; i >= 0; i--)
                _entities[i].Deactivate();

            _entities.Clear();
            _releaseQueue.Clear();
            _update.Clear();
            _fixedUpdate.Clear();
        }

        internal void Schedule(Entity owner, IEntitySystem system)
        {
            if (system is IUpdatableSystem updatable)
                _update.Add(new Slot<IUpdatableSystem>(owner, updatable));

            if (system is IFixedUpdatableSystem fixedUpdatable)
                _fixedUpdate.Add(new Slot<IFixedUpdatableSystem>(owner, fixedUpdatable));
        }

        private void FlushReleases()
        {
            if (_releaseQueue.Count == 0)
                return;
            
            for (int i = 0; i < _releaseQueue.Count; i++)
            {
                Entity entity = _releaseQueue[i];

                RemoveFromEntities(entity);
                entity.Deactivate();
                Released?.Invoke(entity);
            }

            _releaseQueue.Clear();
            
            Compact(_update);
            Compact(_fixedUpdate);
        }

        private void RemoveFromEntities(Entity entity)
        {
            int index = entity.WorldIndex;
            int last = _entities.Count - 1;

            Entity moved = _entities[last];
            _entities[index] = moved;
            moved.WorldIndex = index;

            _entities.RemoveAt(last);
            entity.WorldIndex = -1;
        }

        private static void Compact<T>(List<Slot<T>> list)
        {
            int write = 0;

            for (int read = 0; read < list.Count; read++)
            {
                Slot<T> slot = list[read];

                if (slot.Owner.State == EntityState.Disposed)
                    continue;

                list[write++] = slot;
            }

            list.RemoveRange(write, list.Count - write);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EntityWorld));
        }

        private readonly struct Slot<T>
        {
            public readonly Entity Owner;
            public readonly T System;

            public Slot(Entity owner, T system)
            {
                Owner = owner;
                System = system;
            }
        }
    }
}
