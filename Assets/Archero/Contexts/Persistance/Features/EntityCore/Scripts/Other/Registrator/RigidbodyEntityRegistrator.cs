using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public class RigidbodyEntityRegistrator : MonoEntityRegistrator
    {
        public override void Register(Entity entity)
        {
            entity.AddRigidbody(GetComponent<Rigidbody>());
        }
    }
}