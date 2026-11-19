using Gameplay.Features.GameplayStateMachine;

namespace Gameplay.Features.AI
{
    public class AiParallelState : ParallelState<IUpdatableState>, IUpdatableState
    {
        public AiParallelState(params  IUpdatableState[] states) : base(states)
        {
            
        }

        public void Update(float deltaTime)
        {
            foreach (var state in States)
            {
                state.Update(deltaTime);
            }
        }
    }
}