using System;
using System.Collections.Generic;
using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.ApplyDamage;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class SplashAttackSystem : IInitializableSystem, IDisposableSystem
    {
        private Entity _self;
        private ReactiveEvent _attackDelayEndEvent;
        private Transform _center;
        private ReactiveVariable<float> _radius;
        private ReactiveVariable<float> _damage;
        private LayerMask _targetMask;
        private Buffer<Collider> _hitColliders;

        private List<Entity> _hitEntities;

        private IDisposable _attackDelayEndDisposable;

        private readonly IColliderRegistryService _colliderRegistryService;

        public SplashAttackSystem(IColliderRegistryService colliderRegistryService)
        {
            _colliderRegistryService = colliderRegistryService;
        }

        public void OnInit(Entity entity)
        {
            _self = entity;
            _attackDelayEndEvent = entity.Get<AttackDelayEndEvent>();
            _center = entity.Get<BodyTransform>().Transform;
            _radius = entity.Get<SplashAttackRadius>();
            _damage = entity.Get<SplashAttackDamage>();
            _targetMask = entity.Get<SplashTargetMask>().Value;
            _hitColliders = entity.Get<SplashHitCollidersBuffer>();

            _hitEntities = new List<Entity>(_hitColliders.Items.Length);

            _attackDelayEndDisposable = _attackDelayEndEvent.Subscribe(OnAttackDelayEnd);
        }

        public void OnDispose()
        {
            _attackDelayEndDisposable?.Dispose();
        }

        private void OnAttackDelayEnd()
        {
            Debug.Log($"SPLASH ATTACK");
            _hitColliders.Count = Physics.OverlapSphereNonAlloc(
                _center.position,
                _radius.Value,
                _hitColliders.Items,
                _targetMask,
                QueryTriggerInteraction.Ignore);

            _hitEntities.Clear();

            for (int i = 0; i < _hitColliders.Count; i++)
            {
                if (!_colliderRegistryService.TryGetEntity(_hitColliders.Items[i], out Entity target))
                    continue;

                if (target == _self)
                    continue;

                if (_hitEntities.Contains(target))
                    continue;

                _hitEntities.Add(target);

                if (target.Has<TakeDamageRequest>())
                {
                    target.Get<TakeDamageRequest>().Invoke(_damage.Value);
                    Debug.Log($"SPLASH ATTACH ENTITY {target} with damage {_damage}");
                }
            }
        }
    }
}