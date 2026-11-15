using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public interface IEntityFactory
    {
        public Entity Create(EntityType type, Vector3 position);
        public Entity Create<TArgs>(EntityType type, Vector3 position, TArgs args);
    }
}
