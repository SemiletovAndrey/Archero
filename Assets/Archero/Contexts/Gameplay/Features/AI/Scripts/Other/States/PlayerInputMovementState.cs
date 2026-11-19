using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.GameplayStateMachine;
using Gameplay.Features.MovementFeature;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class PlayerInputMovementState : State, IUpdatableState
    {
        private readonly ReactiveVariable<Vector3> _movementDirection;
        private readonly ReactiveVariable<Vector3> _rotationDirection;

        public PlayerInputMovementState(Entity entity)
        {
            _movementDirection = entity.Get<MoveDirection>();
            _rotationDirection = entity.Get<RotationDirection>();
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public void Update(float deltaTime)
        {
            Vector3 direction = _movementDirection.Value;
            
            if (direction.sqrMagnitude > 0f)
                _rotationDirection.Value = direction;
        }
    }
}