using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.LifeCycle;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.MovementFeature
{
    public class RigidbodyMovementSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private Rigidbody _rigidbody;
        private ReactiveVariable<Vector3> _direction;
        private ReactiveVariable<float> _speed;
        private ReactiveVariable<bool> _isMoving;
        private ICompositeCondition  _canMove;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Get<Rigidbody>();
            _direction = entity.Get<MoveDirection>();
            _speed = entity.Get<MoveSpeed>();
            _isMoving = entity.Get<IsMoving>();
            _canMove = entity.Get<CanMove>();
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
            if (_canMove.Evaluate() == false)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                return;
            }
            
            Vector3 step = _direction.Value * (_speed.Value * fixedDeltaTime);
            
            _isMoving.Value = step.magnitude > 0;
            
            _rigidbody.MovePosition(_rigidbody.position + step);
        }
    }
}