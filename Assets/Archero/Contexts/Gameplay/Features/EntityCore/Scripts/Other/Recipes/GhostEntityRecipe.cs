using Contexts.Scripts.Utilities;
using Gameplay.Features.ApplyDamage;
using Gameplay.Features.ContactTakeDamage;
using Gameplay.Features.LifeCycle;
using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Features.Config.Local;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Scripts.Other.Recipes
{
    public sealed class GhostEntityRecipe : IEntityRecipe
    {
        private IColliderRegistryService _colliderRegistryService;

        private GhostEntityConfig _ghostEntityConfig;

        public GhostEntityRecipe(IColliderRegistryService colliderRegistryService, IEntityConfigService entityConfigService)
        {
            _colliderRegistryService = colliderRegistryService;
            
            _ghostEntityConfig = entityConfigService.Get<GhostEntityConfig>(Type);
        }

        public EntityType Type => EntityType.Ghost;
        
        public void Build(Entity entity)
        {
            entity
                .Add(new MoveDirection())
                .Add(new MoveSpeed(_ghostEntityConfig.MoveSpeed))
                .Add(new RotationDirection())
                .Add(new RotationSpeed(_ghostEntityConfig.RotationSpeed))
                .Add(new IsMoving())
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new RigidbodyRotationSystem())
                .AddSystem(new BodyContactsDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_colliderRegistryService));
            
            entity
                .Add(new MaxHealth(_ghostEntityConfig.MaxHealth))
                .Add(new CurrentHealth(_ghostEntityConfig.MaxHealth))
                .Add(new IsDead(false))
                .Add(new InDeathProcess())
                .Add(new DeathProcessInitialTime(_ghostEntityConfig.DeathProcessTime))
                .Add(new DeathProcessCurrentTime())
                .Add(new TakeDamageRequest())
                .Add(new TakeDamageEvent())
                .Add(new ContactDetectingMask(Layers.CharactersMask))
                .Add(new ContactCollidersBuffer(64))
                .Add(new ContactEntitiesBuffer(64))
                .Add(new BodyContactDamage(_ghostEntityConfig.BodyContactDamage))
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DealDamageContactSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem());
            
            IsDead isDead = entity.Get<IsDead>();
            CurrentHealth currentHealth = entity.Get<CurrentHealth>();
            InDeathProcess inDeathProcess = entity.Get<InDeathProcess>();

            CanMove canMove = new CanMove();
            canMove.Add(new FuncCondition(() => !isDead.Value));

            CanRotate canRotate = new CanRotate();
            canRotate.Add(new FuncCondition(() => !isDead.Value));
            
            MustDie mustDie = new MustDie();
            mustDie.Add(new FuncCondition(() => currentHealth.Value <= 0));
            
            MustSelfRelease mustSelfRelease = new MustSelfRelease();
            mustSelfRelease.Add(new FuncCondition(() => isDead.Value))
                .Add(new FuncCondition(() => inDeathProcess.Value == false));
            
            CanApplyDamage canApplyDamage = new CanApplyDamage();
            canApplyDamage.Add(new FuncCondition(() => !isDead.Value));
            
            MustDieTriggers mustDieTriggers = new MustDieTriggers();
            mustDieTriggers.On(currentHealth);

            MustSelfReleaseTriggers mustSelfReleaseTriggers = new MustSelfReleaseTriggers();
            mustSelfReleaseTriggers.On(isDead);
            mustSelfReleaseTriggers.On(inDeathProcess);

            entity
                .Add(canMove)
                .Add(canRotate)
                .Add(mustDie)
                .Add(mustSelfRelease)
                .Add(canApplyDamage)
                .Add(mustDieTriggers)
                .Add(mustSelfReleaseTriggers);
        }
    }
}