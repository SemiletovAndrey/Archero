using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.GameplayStateMachine;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.AI
{
    public class FindTargetState : State, IUpdatableState
    {
        private ITargetSelector _targetSelector;
        private IEntityWorld _entityWorld;
        private ReactiveVariable<Entity> _currentTarget;

        public FindTargetState(ITargetSelector targetSelector, IEntityWorld entityWorld,
            Entity entity)
        {
            _targetSelector = targetSelector;
            _entityWorld = entityWorld;
            _currentTarget = entity.Get<CurrentTarget>();
        }

        public void Update(float deltaTime)
        {
            _currentTarget.Value = _targetSelector.SelectTargetFrom(_entityWorld.Entities);
        }
    }
}