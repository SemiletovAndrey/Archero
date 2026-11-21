using System;
using System.Collections.Generic;
using Archero.Template.Scripts.Utils;
using Gameplay.Features.AI;
using Gameplay.Features.AI.Scripts;
using Gameplay.Features.EntityCore.Features.Energy;
using Gameplay.Features.MovementFeature;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;

namespace Gameplay.Features.Enemy
{
    public class TeleportetBrainBuilder : IBrainBuilder
    {
        private readonly IEntityWorld _entityWorld;

        public TeleportetBrainBuilder(IEntityWorld entityWorld) => _entityWorld = entityWorld;

        public EntityType Type => EntityType.Teleportator;

        public AiStateMachine Build(Entity entity)
        {
            return CreateLowHpEnemyTeleportStateMachine(entity);
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
    }
}