using System;
using System.Collections.Generic;
using Archero.Template.Scripts.Utils;
using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.EntityCore.Features.Attack;
using Gameplay.Features.EntityCore.Features.Energy;
using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Features.NewInputFeature;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class BrainsFactory : IBrainsFactory
    {
        private readonly IAiBrainContext _aiBrainContext;
        private readonly IInputBus _inputBus;
        private readonly IEntityWorld _entityWorld;

        public BrainsFactory(IAiBrainContext aiBrainContext, IInputBus inputBus, IEntityWorld entityWorld)
        {
            _aiBrainContext = aiBrainContext;
            _inputBus = inputBus;
            _entityWorld = entityWorld;
        }

        public StateMachineBrain CreateGhostBrain(Entity entity)
        {
            AiStateMachine stateMachine = CreateRandomMovementStateMachine(entity);
            StateMachineBrain brain = new StateMachineBrain(stateMachine);

            _aiBrainContext.SetFor(entity, brain);

            return brain;
        }
        
        public StateMachineBrain CreateTeleportatorBrain(Entity entity)
        {
            AiStateMachine stateMachine = CreateLowHpEnemyTeleportStateMachine(entity);
            StateMachineBrain brain = new StateMachineBrain(stateMachine);

            _aiBrainContext.SetFor(entity, brain);

            return brain;
        }

        public StateMachineBrain CreateMainHeroBrain(Entity entity, ITargetSelector targetSelector)
        {
            AiStateMachine combatState = CreateAutoAttackStateMachine(entity);

            PlayerInputMovementState movementState = new PlayerInputMovementState(entity);

            ReactiveVariable<Entity> currentTarget = entity.Get<CurrentTarget>();
            ReactiveVariable<Vector3> currentMovement = entity.Get<MoveDirection>();

            ICompositeCondition fromMovementToCombatStateCondition = new CompositeCondition();
            fromMovementToCombatStateCondition
                .Add(new FuncCondition(() => currentTarget.Value != null))
                .Add(new FuncCondition(() => currentMovement.Value == Vector3.zero));

            ICompositeCondition fromCombatToMovementStateCondition = new CompositeCondition(LogicOperation.Or);
            fromCombatToMovementStateCondition.Add(new FuncCondition(() => currentTarget.Value == null))
                .Add(new FuncCondition(() => currentMovement.Value != Vector3.zero));

            AiStateMachine behaviourStateMachine = new AiStateMachine();
            behaviourStateMachine.AddState(movementState);
            behaviourStateMachine.AddState(combatState);

            behaviourStateMachine.AddTransition(movementState, combatState, fromMovementToCombatStateCondition);
            behaviourStateMachine.AddTransition(combatState, movementState, fromCombatToMovementStateCondition);

            ReadInputState readInput = new ReadInputState(entity, _inputBus);
            FindTargetState findTarget = new FindTargetState(targetSelector, _entityWorld, entity);
            AiParallelState parallelState = new AiParallelState(readInput, findTarget, behaviourStateMachine);

            AiStateMachine rootStateMachine = new AiStateMachine();
            rootStateMachine.AddState(parallelState);
            
            StateMachineBrain brain = new StateMachineBrain(rootStateMachine);
            _aiBrainContext.SetFor(entity, brain);

            return brain;
        }

        private AiStateMachine CreateRandomMovementStateMachine(Entity entity)
        {
            List<IDisposable> disposables = new List<IDisposable>();

            RandomMovementState randomMovementState = new RandomMovementState(entity, 0.5f);
            EmptyState emptyState = new EmptyState();

            TimerService movementTimer = new TimerService(2f);
            disposables.Add(movementTimer);
            disposables.Add(randomMovementState.Entered.Subscribe(movementTimer.Restart));

            TimerService idleTimer = new TimerService(3f);
            disposables.Add(idleTimer);
            disposables.Add(emptyState.Entered.Subscribe(idleTimer.Restart));

            FuncCondition movementTimerEndCondition = new FuncCondition(() => movementTimer.IsOver);
            FuncCondition idleTimerEndCondition = new FuncCondition(() => idleTimer.IsOver);

            AiStateMachine stateMachine = new AiStateMachine(disposables);

            stateMachine.AddState(randomMovementState);
            stateMachine.AddState(emptyState);

            stateMachine.AddTransition(randomMovementState, emptyState, movementTimerEndCondition);
            stateMachine.AddTransition(emptyState, randomMovementState, idleTimerEndCondition);

            return stateMachine;
        }

        private AiStateMachine CreateAutoAttackStateMachine(Entity entity)
        {
            RotateToTargetState rotateToTargetState = new RotateToTargetState(entity);

            AttackTriggerState attackTriggerState = new AttackTriggerState(entity);

            ICondition canAttack = entity.Get<CanStartAttack>();
            Transform transform = entity.Get<BodyTransform>().Transform;
            ReactiveVariable<Entity> currentTarget = entity.Get<CurrentTarget>();

            CompositeCondition fromRotateToAttackCondition = new CompositeCondition();
            fromRotateToAttackCondition
                .Add(canAttack)
                .Add(new FuncCondition(() =>
                {
                    Entity target = currentTarget.Value;

                    if (target == null)
                        return false;
                    Transform targetTransform = target.Get<BodyTransform>().Transform;

                    float angelTarget = Quaternion.Angle(transform.rotation,
                        Quaternion.LookRotation(targetTransform.position - transform.position));

                    return angelTarget < 1f;
                }));

            ReactiveVariable<bool> inAttackProcess = entity.Get<InAttackProcess>();

            ICondition fromAttackToRotateStateCondition = new FuncCondition(() => inAttackProcess.Value == false);

            AiStateMachine stateMachine = new AiStateMachine();

            stateMachine.AddState(rotateToTargetState);
            stateMachine.AddState(attackTriggerState);

            stateMachine.AddTransition(rotateToTargetState, attackTriggerState, fromRotateToAttackCondition);
            stateMachine.AddTransition(attackTriggerState, rotateToTargetState, fromAttackToRotateStateCondition);

            return stateMachine;
        }

        private AiStateMachine CreateRandomTeleportStateMachine(Entity entity)
        {
            List<IDisposable> disposables = new List<IDisposable>();
            
            EmptyState idleState = new EmptyState();
            RandomTeleportState teleportState = new RandomTeleportState(entity);

            CompositeCondition canTeleport = entity.Get<CanTeleport>();
            CompositeCondition canSpendEnergy = entity.Get<CanSpendEnergy>();

            TimerService idleTimer = new TimerService(4f);
            disposables.Add(idleTimer);
            disposables.Add(idleState.Entered.Subscribe(idleTimer.Restart));

            FuncCondition idleToTeleport = new FuncCondition(() => idleTimer.IsOver && canTeleport.Evaluate() && canSpendEnergy.Evaluate());
            FuncCondition teleportToIdle = new FuncCondition(() => true);

            AiStateMachine stateMachine = new AiStateMachine(disposables);

            stateMachine.AddState(idleState);
            stateMachine.AddState(teleportState);

            stateMachine.AddTransition(idleState, teleportState, idleToTeleport);
            stateMachine.AddTransition(teleportState, idleState, teleportToIdle);

            return stateMachine;
        }

        private AiStateMachine CreateLowHpEnemyTeleportStateMachine(Entity entity)
        {
            List<IDisposable> disposables = new List<IDisposable>();
            
            CurrentEnergy currentEnergy = entity.Get<CurrentEnergy>();
            MaxEnergy maxEnergy = entity.Get<MaxEnergy>();
            CompositeCondition canTeleport = entity.Get<CanTeleport>();

            const float MinEnergyPercent = 0.4f;
            
            EmptyState idleState = new EmptyState();
            LowestHealthTargetSelector selector = new LowestHealthTargetSelector(entity);
            TeleportToTargetState teleportState = new TeleportToTargetState(selector, _entityWorld, entity, 1.5f);

            TimerService idleTimer = new TimerService(4f);
            disposables.Add(idleTimer);
            disposables.Add(idleState.Entered.Subscribe(idleTimer.Restart));

            FuncCondition idleToTeleport = new FuncCondition(() =>
                idleTimer.IsOver
                && currentEnergy.Value >= maxEnergy.Value * MinEnergyPercent
                && canTeleport.Evaluate());
            FuncCondition teleportToIdle = new FuncCondition(() => true);

            AiStateMachine stateMachine = new AiStateMachine(disposables);

            stateMachine.AddState(idleState);
            stateMachine.AddState(teleportState);

            stateMachine.AddTransition(idleState, teleportState, idleToTeleport);
            stateMachine.AddTransition(teleportState, idleState, teleportToIdle);

            return stateMachine;
        }
    }
}