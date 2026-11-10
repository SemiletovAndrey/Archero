using Gameplay.Features.LifeCycle;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.MovementFeature
{
    public sealed class GhostEntityRecipe : IEntityRecipe
    {
        public EntityType Type => EntityType.Ghost;
        public void Build(Entity entity)
        {
            entity
                .Add(new MoveDirection())
                .Add(new MoveSpeed(10))
                .Add(new RotationDirection())
                .Add(new RotationSpeed(900))
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new RigidbodyRotationSystem())
                
                .Add(new MaxHealth(100))
                .Add(new CurrentHealth(100))
                .Add(new IsDead(false))
                .AddSystem(new DeathSystem());
        }
    }
}