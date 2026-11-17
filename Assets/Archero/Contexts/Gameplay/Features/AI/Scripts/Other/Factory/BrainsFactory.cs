using System;
using System.Collections.Generic;
using Archero.Template.Scripts.Utils;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;

namespace Gameplay.Features.AI
{
    public class BrainsFactory
    {
        public AiStateMachine CreateRandomMovementStateMachine(Entity entity)
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
    }
}