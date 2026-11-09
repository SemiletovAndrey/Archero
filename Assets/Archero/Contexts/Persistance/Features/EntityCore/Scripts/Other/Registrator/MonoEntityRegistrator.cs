using UnityEngine;

namespace Persistance.Features.EntityCore
{
    public abstract class MonoEntityRegistrator : MonoBehaviour
    {
        public abstract void Register(Entity entity);
    }
}