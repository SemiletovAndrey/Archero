using Contexts.Scripts.Utilities;
using Gameplay.Features.ContactTakeDamage;
using Gameplay.Features.LifeCycle;
using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Scripts.Other.Recipes
{
    public sealed class ProjectileEntityRecipe : IEntityRecipe<ProjectileArgs>
    {
        public EntityType Type => EntityType.Projectile;
        
        private IColliderRegistryService _colliderRegistryService;

        public ProjectileEntityRecipe(IColliderRegistryService colliderRegistryService)
        {
            _colliderRegistryService = colliderRegistryService;
        }

        public void Build(Entity entity, ProjectileArgs args)
        {
            entity
                .Add(new MoveDirection(args.Direction))
                .Add(new MoveSpeed(10))
                .Add(new RotationDirection(args.Direction))
                .Add(new RotationSpeed(9999))
                .Add(new IsMoving())
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new RigidbodyRotationSystem())
                .AddSystem(new BodyContactsDetectingSystem())
                .AddSystem(new BodyContactsEntitiesFilterSystem(_colliderRegistryService));
            
            entity
                .Add(new IsDead(false))
                .Add(new InDeathProcess())
                .Add(new ContactDetectingMask(Layers.CharactersMask))
                .Add(new ContactCollidersBuffer(64))
                .Add(new ContactEntitiesBuffer(64))
                .Add(new BodyContactDamage(args.Damage))
                .Add(new DeathMask(Layers.CharactersMask))
                .Add(new IsTouchDeathMask())
                .AddSystem(new DeathSystem())
                .AddSystem(new DealDamageContactSystem())
                .AddSystem(new DeathMaskTouchDetectorSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem());
            
            IsDead isDead = entity.Get<IsDead>();
            IsTouchDeathMask isTouchDeathMask = entity.Get<IsTouchDeathMask>();

            CanMove canMove = new CanMove();
            canMove.Add(new FuncCondition(() => !isDead.Value));

            CanRotate canRotate = new CanRotate();
            canRotate.Add(new FuncCondition(() => !isDead.Value));
            
            MustDie mustDie = new MustDie();
            mustDie.Add(new FuncCondition(() => isTouchDeathMask.Value));
            
            MustSelfRelease mustSelfRelease = new MustSelfRelease();
            mustSelfRelease.Add(new FuncCondition(() => isDead.Value));
            
            MustDieTriggers mustDieTriggers = new MustDieTriggers();
            mustDieTriggers.On(isTouchDeathMask);

            MustSelfReleaseTriggers mustSelfReleaseTriggers = new MustSelfReleaseTriggers();
            mustSelfReleaseTriggers.On(isDead);

            entity
                .Add(canMove)
                .Add(canRotate)
                .Add(mustDie)
                .Add(mustSelfRelease)
                .Add(mustDieTriggers)
                .Add(mustSelfReleaseTriggers);
        }
    }
}