using System;
using System.Collections.Generic;

namespace Persistance.Features.EntityCore
{
    public class EntityLifeService : IEntityLifeService, IDisposable
    {
        public event Action<Entity> Added;
        public event Action<Entity> Released;
        
        private readonly List<Entity> _entities = new List<Entity>();
        private readonly List<Entity> _releaseRequest = new List<Entity>();

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);

            entity.Init();
            
            Added?.Invoke(entity);
        }

        public void Update(float deltaTime)
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].OnUpdate(deltaTime);
            }

            foreach (Entity entity in _releaseRequest)
            {
                _entities.Remove(entity);
                entity.Dispose();
                Released?.Invoke(entity);
            }

            _releaseRequest.Clear();
        }

        public void Release(Entity entity)
        {
            _releaseRequest.Add(entity);
        }

        public void Dispose()
        {
            foreach (Entity entity in _entities)
            {
                entity.Dispose();
            }
            
            _entities.Clear();
            _releaseRequest.Clear();
        }
    }
}