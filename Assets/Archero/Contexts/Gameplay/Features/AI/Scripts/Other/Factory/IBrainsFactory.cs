using Persistance.Features.EntityCore;

namespace Gameplay.Features.AI
{
    public interface IBrainsFactory
    {
        public StateMachineBrain CreateGhostBrain(Entity entity);
        StateMachineBrain CreateMainHeroBrain(Entity entity, ITargetSelector targetSelector);
        StateMachineBrain CreateTeleportatorBrain(Entity entity);
    }
}