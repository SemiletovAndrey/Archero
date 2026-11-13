using System;
using System.Collections.Generic;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public class Registration: IDisposable
    {
        private readonly Dictionary<Collider, Entity> _map;
        private readonly IReadOnlyList<Collider> _colliders;

        public Registration(Dictionary<Collider, Entity> map, IReadOnlyList<Collider> colliders)
        {
            _map = map;
            _colliders = colliders;
        }

        public void Dispose()
        {
            foreach (Collider collider in _colliders)
                _map.Remove(collider);
        }
    }
}