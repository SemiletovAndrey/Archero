using Gameplay.Features.ApplyDamage;
using Gameplay.Features.ContactTakeDamage;
using Gameplay.Features.LifeCycle;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.MovementFeature
{
    public class HeroEntityRecipe : IEntityRecipe
    {
        public EntityType Type => EntityType.Hero;
        
        private IColliderRegistryService _colliderRegistryService;

        public HeroEntityRecipe(IColliderRegistryService colliderRegistryService)
        {
            _colliderRegistryService = colliderRegistryService;
        }

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
                .Add(new TakeDamageRequest())
                .Add(new TakeDamageEvent())
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new SelfReleaseSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem());
            
            var isDead = entity.Get<IsDead>();
            var currentHealth = entity.Get<CurrentHealth>();

            CanMove canMove = new CanMove();
            canMove.Add(new FuncCondition(() => !isDead.Value));

            CanRotate canRotate = new CanRotate();
            canRotate.Add(new FuncCondition(() => !isDead.Value));
            
            MustDie mustDie = new MustDie();
            mustDie.Add(new FuncCondition(() => currentHealth.Value <= 0));
            
            MustSelfRelease mustSelfRelease = new MustSelfRelease();
            mustSelfRelease.Add(new FuncCondition(() => isDead.Value))
                .Add(new FuncCondition(() => entity.Get<InDeathProcess>().Value == false));
            
            CanApplyDamage canApplyDamage = new CanApplyDamage();
            canApplyDamage.Add(new FuncCondition(() => !isDead.Value));

            entity
                .Add(canMove)
                .Add(canRotate)
                .Add(mustDie)
                .Add(mustSelfRelease)
                .Add(canApplyDamage);
        }
    }
}