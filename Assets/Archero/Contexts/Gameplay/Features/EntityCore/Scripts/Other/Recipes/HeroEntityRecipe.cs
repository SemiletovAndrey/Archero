using Gameplay.Features.ApplyDamage;
using Gameplay.Features.EntityCore.Features.Attack;
using Gameplay.Features.LifeCycle;
using Gameplay.Features.MovementFeature;
using Persistance.Features.EntityCore;
using Template.Features.Config.Local;
using Template.Scripts.Utils;

namespace Gameplay.Features.EntityCore.Features.Scripts.Other.Recipes
{
    public class HeroEntityRecipe : IEntityRecipe
    {
        private readonly IEntityFactory _entityFactory;

        private HeroEntityConfig _heroEntityConfig;
        
        public HeroEntityRecipe(IEntityFactory entityFactory, IEntityConfigService entityConfigService)
        {
            _entityFactory = entityFactory;
            
            _heroEntityConfig = entityConfigService.Get<HeroEntityConfig>(Type);
        }

        public EntityType Type => EntityType.Hero;

        public void Build(Entity entity)
        {
            //Movement
            entity
                .Add(new MoveDirection())
                .Add(new MoveSpeed(_heroEntityConfig.MoveSpeed))
                .Add(new RotationDirection())
                .Add(new RotationSpeed(_heroEntityConfig.RotationSpeed))
                .Add(new IsMoving())
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new RigidbodyRotationSystem());
            
            //Health
            entity
                .Add(new MaxHealth(_heroEntityConfig.MaxHealth))
                .Add(new CurrentHealth(_heroEntityConfig.MaxHealth))
                .Add(new IsDead(false))
                .Add(new InDeathProcess())
                .Add(new DeathProcessInitialTime(_heroEntityConfig.DeathProcessInitialTime))
                .Add(new DeathProcessCurrentTime())
                .Add(new TakeDamageRequest())
                .Add(new TakeDamageEvent())
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DeathSystem())
                .AddSystem(new DeathProcessTimerSystem())
                .AddSystem(new DisableCollidersOnDeathSystem())
                .AddSystem(new SelfReleaseSystem());
            
            //Attack
            entity
                .Add(new AttackProcessInitialTime(_heroEntityConfig.AttackProcessTime))
                .Add(new AttackProcessCurrentTime())
                .Add(new InAttackProcess())
                .Add(new AttackDelayTime(_heroEntityConfig.AttackDelayTime))
                .Add(new StartAttackRequest())
                .Add(new InstantAttackDamage(_heroEntityConfig.InstantAttackDamage))
                .Add(new StartAttackEvent())
                .Add(new EndAttackEvent())
                .Add(new AttackDelayEndEvent())
                .Add(new AttackCancelEvent())
                .Add(new AttackCooldownInitialTime(_heroEntityConfig.AttackCooldown))
                .Add(new AttackCooldownCurrentTime())
                .Add(new InAttackCooldown())
                .AddSystem(new StartAttackSystem())
                .AddSystem(new AttackProcessTimerSystem())
                .AddSystem(new EndAttackSystem())
                .AddSystem(new AttackDelayEndTriggerSystem())
                .AddSystem(new InstantsShootSystem(_entityFactory))
                .AddSystem(new AttackCancelSystem())
                .AddSystem(new AttackCooldownTimerSystem());
            
            IsDead isDead = entity.Get<IsDead>();
            IsMoving isMoving = entity.Get<IsMoving>();
            CurrentHealth currentHealth = entity.Get<CurrentHealth>();
            InAttackProcess inAttackProcess = entity.Get<InAttackProcess>();
            InAttackCooldown inAttackCooldown = entity.Get<InAttackCooldown>();
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

            CanStartAttack canStartAttack = new CanStartAttack();
            canStartAttack.Add(new FuncCondition(() => !isDead.Value));
            canStartAttack.Add(new FuncCondition(() => !inAttackProcess.Value));
            canStartAttack.Add(new FuncCondition(() => !isMoving.Value));
            canStartAttack.Add(new FuncCondition(() => !inAttackCooldown.Value));
            
            MustCancelAttack mustCancelAttack = new MustCancelAttack(LogicOperation.Or);
            mustCancelAttack.Add(new FuncCondition(() => isDead.Value));
            mustCancelAttack.Add(new FuncCondition(() => isMoving.Value));
            
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
                .Add(canStartAttack)
                .Add(mustCancelAttack)
                .Add(mustDieTriggers)
                .Add(mustSelfReleaseTriggers);
        }
    }
}