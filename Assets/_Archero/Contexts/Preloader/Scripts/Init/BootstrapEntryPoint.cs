using Template.Features.GameStateMachine;
using Zenject;

namespace DomainOfBattles.Contexts.Preloader
{
    public class BootstrapEntryPoint : IInitializable
    {
        private readonly IGameStateMachine _stateMachine;
 
        public BootstrapEntryPoint(IGameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
 
        public void Initialize()
        {
            _stateMachine.Enter<BootState>();
        }

    }
}