using System;
using Persistance.Features.GameLoopFeature;

namespace Persistance.Features.EntityCore
{
    public interface IEntityWorld : IGameUpdatable, IGameFixedUpdatable
    {
        public event Action<Entity> Added;
        public event Action<Entity> Released;

        public int Count { get; }

        public Entity Create(EntityType type);

        public void Add(Entity entity);

        public void Release(Entity entity);
    }
}
