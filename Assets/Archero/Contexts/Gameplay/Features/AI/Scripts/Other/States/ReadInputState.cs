using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.GameplayStateMachine;
using Gameplay.Features.MovementFeature;
using Persistance.Features.EntityCore;
using Template.Features.NewInputFeature;
using Template.Features.NewInputFeature.Scripts.Example;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class ReadInputState : State, IUpdatableState
    {
        private readonly IInputBus _bus;
        private readonly ReactiveVariable<Vector3> _movementDirection;

        private Vector2 _currentInput;

        public ReadInputState(Entity entity, IInputBus bus)
        {
            _bus = bus;
            _movementDirection = entity.Get<MoveDirection>();
        }

        public override void Enter()
        {
            base.Enter();
            _bus.Subscribe<MovementSignal>(OnMovementReceived);
        }

        public override void Exit()
        {
            base.Exit();
            _bus.Unsubscribe<MovementSignal>(OnMovementReceived);

            _currentInput = Vector2.zero;
            _movementDirection.Value = Vector3.zero;
        }

        public void Update(float deltaTime)
        {
            _movementDirection.Value = new Vector3(_currentInput.x, 0, _currentInput.y);
        }

        private void OnMovementReceived(MovementSignal signal)
        {
            _currentInput = signal.Value;
        }
    }
}