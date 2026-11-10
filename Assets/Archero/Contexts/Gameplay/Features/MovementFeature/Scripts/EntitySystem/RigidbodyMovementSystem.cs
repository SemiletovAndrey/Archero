using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.MovementFeature
{
    public class RigidbodyMovementSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private Rigidbody _rigidbody;
        private ReactiveVariable<Vector3> _direction;
        private ReactiveVariable<float> _speed;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Get<Rigidbody>();
            _direction = entity.Get<MoveDirection>();
            _speed = entity.Get<MoveSpeed>();
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
            Vector3 step = _direction.Value * (_speed.Value * fixedDeltaTime);
            _rigidbody.MovePosition(_rigidbody.position + step);
        }
    }
}