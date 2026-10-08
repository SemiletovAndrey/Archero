using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.GameStateMachine
{
    public interface IGameStateMachine
    {
        public Type CurrentStateType { get; }
        
        public bool IsTransitioning { get; }

        public void Enter<TState>() where TState : class, IState;
        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>;   
    }
}