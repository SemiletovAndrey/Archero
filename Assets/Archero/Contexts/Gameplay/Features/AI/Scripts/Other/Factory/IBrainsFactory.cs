using Persistance.Features.EntityCore;

namespace Gameplay.Features.AI
{
    public interface IBrainsFactory
    {
        public StateMachineBrain Create(Entity entity, EntityType type);
    }
}