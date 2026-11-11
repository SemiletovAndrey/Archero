using Gameplay.Features.LifeCycle;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;

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
                .AddSystem(new RigidbodyRotationSystem());
            
            entity
                .Add(new MaxHealth(100))
                .Add(new CurrentHealth(100))
                .Add(new IsDead(false))
                .Add(new InDeathProcess())
                .Add(new DeathProcessInitialTime(2))
                .Add(new DeathProcessCurrentTime())
                .AddSystem(new DeathSystem())
                .AddSystem(new SelfReleaseSystem())
                .AddSystem(new DeathProcessTimerSystem());
            
            var isDead = entity.Get<IsDead>();

            CanMove canMove = new CanMove();
            canMove.Add(new FuncCondition(() => !isDead.Value));

            CanRotate canRotate = new CanRotate();
            canRotate.Add(new FuncCondition(() => !isDead.Value));

            entity
                .Add(canMove)
                .Add(canRotate);
        }
    }
}