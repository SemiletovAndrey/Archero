using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public interface IColliderRegistryService
    {
        public bool TryGetEntity(Collider collider, out Entity entity);
    }
}