using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.LifeCycle;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.MovementFeature
{
    public class RigidbodyRotationSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private Rigidbody _rigidbody;
        
        private ReactiveVariable<float> _rotationSpeed;
        private ReactiveVariable<Vector3> _direction;
        private ICompositeCondition _canRotate;
        
        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Get<Rigidbody>();
            
            _rotationSpeed = entity.Get<RotationSpeed>();
            _direction = entity.Get<RotationDirection>();

            _canRotate = entity.Get<CanRotate>();
        }

        public void OnFixedUpdate(float fixedDeltaTime)
        {
            if(_canRotate.Evaluate() == false)
                return;
            
            if(_direction.Value == Vector3.zero)
                return;
            
            Quaternion lookRotation = Quaternion.LookRotation(_direction.Value.normalized);
            
            float step = _rotationSpeed.Value * fixedDeltaTime;
            
            Quaternion rotation = Quaternion.RotateTowards(_rigidbody.rotation, lookRotation, step);
            
            _rigidbody.MoveRotation(rotation);
        }
    }
}