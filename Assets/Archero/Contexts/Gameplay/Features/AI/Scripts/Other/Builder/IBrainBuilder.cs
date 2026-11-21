using Persistance.Features.EntityCore;

namespace Gameplay.Features.AI.Scripts
{
    public interface IBrainBuilder
    {
        public EntityType Type { get; }
        public AiStateMachine Build(Entity entity);
    }
}