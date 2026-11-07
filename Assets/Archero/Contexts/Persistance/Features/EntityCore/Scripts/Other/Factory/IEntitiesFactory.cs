using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public interface IEntitiesFactory
    {
        public Entity CreateTestEntity(Vector3 position);
    }
}