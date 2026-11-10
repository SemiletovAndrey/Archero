using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public interface IEntityFactory
    {
        public Entity Create(EntityType type, Vector3 position);
    }
}
