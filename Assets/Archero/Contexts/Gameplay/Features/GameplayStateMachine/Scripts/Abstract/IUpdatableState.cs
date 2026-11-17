namespace Gameplay.Features.GameplayStateMachine
{
    public interface IUpdatableState : IState
    {
        public void Update(float deltaTime);
    }
}