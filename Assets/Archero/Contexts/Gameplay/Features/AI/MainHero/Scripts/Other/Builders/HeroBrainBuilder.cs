using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.AI.Scripts;
using Gameplay.Features.EntityCore.Features.Attack;
using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Features.NewInputFeature;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.AI.MainHero
{
    public class HeroBrainBuilder : IBrainBuilder
    {
        private const float AttackAngleThreshold = 3f;

        private readonly IInputBus _inputBus;
        private readonly IEntityWorld _entityWorld;

        public HeroBrainBuilder(
            IInputBus inputBus,
            IEntityWorld entityWorld)
        {
            _inputBus = inputBus;
            _entityWorld = entityWorld;
            
        }

        public EntityType Type => EntityType.Hero;

        public AiStateMachine Build(Entity entity)
        {
            ITargetSelector targetSelector = new NearestDamageableTargetSelector(entity);
            ReadInputState readInput = new ReadInputState(entity, _inputBus);
            FindTargetState findTarget = new FindTargetState(targetSelector, _entityWorld, entity);
            AiStateMachine behaviour = CreateMovementOrCombat(entity);

            AiParallelState parallelState = new AiParallelState(readInput, findTarget, behaviour);

            AiStateMachine root = new AiStateMachine();
            root.AddState(parallelState);

            return root;
        }

        private AiStateMachine CreateMovementOrCombat(Entity entity)
        {
            ReactiveVariable<Entity> currentTarget = entity.Get<CurrentTarget>();
            ReactiveVariable<Vector3> currentMovement = entity.Get<MoveDirection>();

            PlayerInputMovementState movementState = new PlayerInputMovementState(entity);
            AiStateMachine combatState = CreateAutoAttack(entity);

            ICompositeCondition movementToCombat = new CompositeCondition();
            movementToCombat
                .Add(new FuncCondition(() => currentTarget.Value != null))
                .Add(new FuncCondition(() => currentMovement.Value == Vector3.zero));

            ICompositeCondition combatToMovement = new CompositeCondition(LogicOperation.Or);
            combatToMovement
                .Add(new FuncCondition(() => currentTarget.Value == null))
                .Add(new FuncCondition(() => currentMovement.Value != Vector3.zero));

            AiStateMachine machine = new AiStateMachine();
            machine.AddState(movementState);
            machine.AddState(combatState);
            machine.AddTransition(movementState, combatState, movementToCombat);
            machine.AddTransition(combatState, movementState, combatToMovement);

            return machine;
        }

        private AiStateMachine CreateAutoAttack(Entity entity)
        {
            RotateToTargetState rotateState = new RotateToTargetState(entity);
            AttackTriggerState attackState = new AttackTriggerState(entity);

            ICondition canAttack = entity.Get<CanStartAttack>();
            Transform transform = entity.Get<BodyTransform>().Transform;
            ReactiveVariable<Entity> currentTarget = entity.Get<CurrentTarget>();
            ReactiveVariable<bool> inAttackProcess = entity.Get<InAttackProcess>();

            CompositeCondition rotateToAttack = new CompositeCondition();
            rotateToAttack
                .Add(canAttack)
                .Add(new FuncCondition(() => IsLookingAtTarget(transform, currentTarget.Value)));

            ICondition attackToRotate = new FuncCondition(() => !inAttackProcess.Value);

            AiStateMachine machine = new AiStateMachine();
            machine.AddState(rotateState);
            machine.AddState(attackState);
            machine.AddTransition(rotateState, attackState, rotateToAttack);
            machine.AddTransition(attackState, rotateState, attackToRotate);

            return machine;
        }

        private static bool IsLookingAtTarget(Transform self, Entity target)
        {
            if (target == null)
                return false;

            Transform targetTransform = target.Get<BodyTransform>().Transform;
            Quaternion toTarget = Quaternion.LookRotation(targetTransform.position - self.position);

            return Quaternion.Angle(self.rotation, toTarget) < AttackAngleThreshold;
        }

    }
}