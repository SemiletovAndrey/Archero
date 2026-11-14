using Gameplay.Features.ApplyDamage;
using Gameplay.Features.EntityCore.Features.Attack;
using Gameplay.Features.LifeCycle;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;

namespace Gameplay.Features.MovementFeature
{
    public class HeroEntityRecipe : IEntityRecipe
    {
        public EntityType Type => EntityType.Hero;

        public void Build(Entity entity)
        {
            //Movement
            entity
                .Add(new MoveDirection())
                .Add(new MoveSpeed(10))
                .Add(new RotationDirection())
                .Add(new RotationSpeed(900))
                .Add(new IsMoving())
                .AddSystem(new RigidbodyMovementSystem())
                .AddSystem(new RigidbodyRotationSystem());
            
            //Health
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
            
            //Attack
            entity
                .Add(new AttackProcessInitialTime(3))
                .Add(new AttackProcessCurrentTime())
                .Add(new InAttackProcess())
                .Add(new AttackDelayTime(1))
                .Add(new StartAttackRequest())
                .Add(new InstantAttackDamage(50))
                .Add(new StartAttackEvent())
                .Add(new EndAttackEvent())
                .Add(new AttackDelayEndEvent())
                .AddSystem(new StartAttackSystem())
                .AddSystem(new AttackProcessTimerSystem())
                .AddSystem(new EndAttackSystem())
                .AddSystem(new AttackDelayEndTriggerSystem())
                .AddSystem(new InstantsShootSystem());
            
            IsDead isDead = entity.Get<IsDead>();
            IsMoving isMoving = entity.Get<IsMoving>();
            CurrentHealth currentHealth = entity.Get<CurrentHealth>();
            InAttackProcess inAttackProcess = entity.Get<InAttackProcess>();

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

            CanStartAttack canStartAttack = new CanStartAttack();
            canStartAttack.Add(new FuncCondition(() => !isDead.Value));
            canStartAttack.Add(new FuncCondition(() => !inAttackProcess.Value));
            canStartAttack.Add(new FuncCondition(() => !isMoving.Value));

            entity
                .Add(canMove)
                .Add(canRotate)
                .Add(mustDie)
                .Add(mustSelfRelease)
                .Add(canApplyDamage)
                .Add(canStartAttack);
        }
    }
}