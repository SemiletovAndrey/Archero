using System;
using System.Collections.Generic;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public class ColliderRegistryService : IColliderRegistryService, IDisposable
    {
        private readonly IEntityWorld _world;
        private readonly Dictionary<Collider, Entity> _map = new();

        public ColliderRegistryService(IEntityWorld world)
        {
            _world = world;
            _world.Added += OnAdded;
        }

        public void Dispose() => _world.Added -= OnAdded;

        public bool TryGetEntity(Collider collider, out Entity entity) => _map.TryGetValue(collider, out entity);

        private void OnAdded(Entity entity)
        {
            if (!entity.TryGet(out EntityColliders colliders))
                return;

            foreach (Collider collider in colliders.Items)
                _map[collider] = entity;

            entity.Add(new Registration(_map, colliders.Items));
        }
    }
}