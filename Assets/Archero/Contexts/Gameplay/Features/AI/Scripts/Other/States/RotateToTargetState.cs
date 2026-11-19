using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.GameplayStateMachine;
using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class RotateToTargetState : State, IUpdatableState
    {
        private ReactiveVariable<Vector3> _rotationDirection;
        private ReactiveVariable<Entity> _currentTarget;
        private Transform _transform;

        private BodyTransform _bodyTransform;

        public RotateToTargetState(Entity entity)
        {
            _rotationDirection = entity.Get<RotationDirection>();
            _currentTarget = entity.Get<CurrentTarget>();
            _transform = entity.Get<BodyTransform>().Transform;
        }

        public void Update(float deltaTime)
        {
            if (_currentTarget.Value != null)
            {
                _bodyTransform = _currentTarget.Value.Get<BodyTransform>();

                _rotationDirection.Value = (_bodyTransform.Transform.position - _transform.position).normalized;
            }
        }
    }
}