using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.GameplayStateMachine;
using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class TeleportToTargetState : State, IUpdatableState
    {
        private readonly ITargetSelector _targetSelector;
        private readonly IEntityWorld _entityWorld;
        private readonly ReactiveVariable<Entity> _currentTarget;
        private readonly ReactiveEvent<Vector3> _teleportRequest;
        private readonly ReactiveVariable<float> _teleportRadius;
        private readonly Transform _body;

        private readonly float _stopDistance;

        public TeleportToTargetState(ITargetSelector targetSelector, IEntityWorld entityWorld,
            Entity entity, float stopDistance)
        {
            _targetSelector = targetSelector;
            _entityWorld = entityWorld;
            _currentTarget = entity.Get<CurrentTarget>();
            _teleportRequest = entity.Get<TeleportRequest>();
            _teleportRadius = entity.Get<TeleportRadius>();
            _body = entity.Get<BodyTransform>().Transform;

            _stopDistance = stopDistance;
        }

        public override void Enter()
        {
            base.Enter();

            Entity target = _targetSelector.SelectTargetFrom(_entityWorld.Entities);
            _currentTarget.Value = target;

            if (target == null)
                return;

            Vector3 origin = _body.position;
            Vector3 toTarget = target.Get<BodyTransform>().Transform.position - origin;
            toTarget.y = 0f;

            float distance = toTarget.magnitude;
            float jumpDistance = Mathf.Clamp(distance - _stopDistance, 0f, _teleportRadius.Value);

            Vector3 destination = jumpDistance > 0f
                ? origin + toTarget / distance * jumpDistance
                : origin;

            _teleportRequest.Invoke(destination);
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}