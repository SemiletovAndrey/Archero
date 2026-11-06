using System;
using Persistance.Features.GameLoopFeature;

namespace Persistance.Features.EntityCore
{
    public interface IEntityLifeService : IGameUpdatable
    {
        public event Action<Entity> Added;
        public event Action<Entity> Released;
        
        public void AddEntity(Entity entity);
        public void Release(Entity entity);
    }
}