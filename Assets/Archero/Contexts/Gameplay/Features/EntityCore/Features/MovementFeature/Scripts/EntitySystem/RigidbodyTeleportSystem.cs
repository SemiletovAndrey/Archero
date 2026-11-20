using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Features.MovementFeature
{
    public class RigidbodyTeleportSystem : IInitializableSystem, IDisposableSystem
    {
        private Rigidbody _rigidbody;
        private ReactiveVariable<float> _teleportRadius;
        private ICompositeCondition _canTeleport;
        private ReactiveEvent<Vector3> _teleportRequest;
        private ReactiveEvent _teleportEvent;
        
        private IDisposable _teleportRequestDisposable;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Get<Rigidbody>();
            _teleportRadius = entity.Get<TeleportRadius>();
            _canTeleport = entity.Get<CanTeleport>();
            _teleportRequest = entity.Get<TeleportRequest>();
            _teleportEvent = entity.Get<TeleportEvent>();

            _teleportRequestDisposable = _teleportRequest.Subscribe(OnTeleportRequest);
        }
        
        public void OnDispose()
        {
            _teleportRequestDisposable.Dispose();
        }

        private void OnTeleportRequest(Vector3 target)
        {
            if (!_canTeleport.Evaluate())
                return;

            if (!_rigidbody.isKinematic)
                _rigidbody.linearVelocity = Vector3.zero;

            _rigidbody.position = target;

            _rigidbody.transform.position = target;

            _teleportEvent.Invoke();
        }
        
        private Vector3 GetRandomPointInRadius(Vector3 origin, float radius)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            return new Vector3(origin.x + randomCircle.x, origin.y, origin.z + randomCircle.y);
        }
    }
}