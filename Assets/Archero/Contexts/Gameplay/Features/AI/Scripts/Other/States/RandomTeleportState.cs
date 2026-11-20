using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.GameplayStateMachine;
using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class RandomTeleportState : State, IUpdatableState
    {
        private readonly ReactiveEvent<Vector3> _teleportRequest;
        private readonly ReactiveVariable<float> _teleportRadius;
        private readonly Transform _body;

        public RandomTeleportState(Entity entity)
        {
            _teleportRequest = entity.Get<TeleportRequest>();
            _teleportRadius = entity.Get<TeleportRadius>();
            _body = entity.Get<BodyTransform>().Transform;
        }

        public override void Enter()
        {
            base.Enter();

            _teleportRequest.Invoke(GetRandomPoint());
        }

        public void Update(float deltaTime)
        {
            
        }

        private Vector3 GetRandomPoint()
        {
            Vector2 offset = Random.insideUnitCircle * _teleportRadius.Value;
            Vector3 origin = _body.position;

            return new Vector3(origin.x + offset.x, origin.y, origin.z + offset.y);
        }
    }
}