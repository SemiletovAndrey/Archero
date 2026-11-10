using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.MovementFeature
{
    public class RigidbodyRotationSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private Rigidbody _rigidbody;
        
        private ReactiveVariable<float> _rotationSpeed;
        private ReactiveVariable<Vector3> _direction;
        
        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Get<Rigidbody>();
            
            _rotationSpeed = entity.Get<RotationSpeed>();
            _direction = entity.Get<RotationDirection>();
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
            if(_direction.Value == Vector3.zero)
                return;
            
            Quaternion lookRotation = Quaternion.LookRotation(_direction.Value.normalized);
            
            float step = _rotationSpeed.Value * fixedDeltaTime;
            
            Quaternion rotation = Quaternion.RotateTowards(_rigidbody.rotation, lookRotation, step);
            
            _rigidbody.MoveRotation(rotation);
        }
    }
}