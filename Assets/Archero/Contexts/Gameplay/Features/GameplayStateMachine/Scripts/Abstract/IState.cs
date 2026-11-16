using Contexts.Scripts.Utilities.Reactive;

namespace Gameplay.Features.GameplayStateMachine
{
    public interface IState
    {
        public IReadOnlyEvent Entered { get; } 
        public IReadOnlyEvent Exited { get; } 
            
        public void Enter();
        public void Exit();
    }
}