using UnityEngine;

namespace Persistance.Features.EntityCore
{
    /// <summary>
    /// Нужен только для нетривиальной сборки компонентов из сцены/префаба.
    /// Простые Unity-компоненты (Rigidbody, Animator...) регистрировать не нужно: их достаточно
    /// перетащить в список Exposed у MonoEntity.
    /// </summary>
    public abstract class MonoEntityRegistrator : MonoBehaviour
    {
        public abstract void Register(Entity entity);
    }
}
