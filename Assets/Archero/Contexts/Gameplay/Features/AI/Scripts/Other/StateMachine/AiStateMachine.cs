using System;
using System.Collections.Generic;
using Gameplay.Features.GameplayStateMachine;

namespace Gameplay.Features.AI
{
    public class AiStateMachine : StateMachine<IUpdatableState>
    {
        public AiStateMachine() : base(new List<IDisposable>())
        {
            
        }
        public AiStateMachine(List<IDisposable> disposables) : base(disposables)
        {
        }

        protected override void UpdateLogic(float deltaTime)
        {
            base.UpdateLogic(deltaTime);
            
            CurrentState?.Update(deltaTime);
        }
    }
}