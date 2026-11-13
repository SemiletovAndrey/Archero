using System;
using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.LifeCycle;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.ApplyDamage
{
    public class ApplyDamageSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveEvent<float> _damageRequest;
        private ReactiveEvent<float> _damageEvent;
        
        private ReactiveVariable<float> _health;
        
        private CompositeCondition _canApplyDamage;

        private IDisposable _requestDisposable; 

        public void OnInit(Entity entity)
        {
            _damageRequest = entity.Get<TakeDamageRequest>();
            _damageEvent = entity.Get<TakeDamageEvent>();
            _health = entity.Get<CurrentHealth>();
            _canApplyDamage = entity.Get<CanApplyDamage>();

            _requestDisposable = _damageRequest.Subscribe(OnDamageRequest);
        }
        
        public void OnDispose()
        {
            _requestDisposable?.Dispose();
        }

        private void OnDamageRequest(float damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage));
            
            if(!_canApplyDamage.Evaluate())
                return;
            
            _health.Value = MathF.Max(_health.Value - damage, 0);
            _damageEvent.Invoke(damage);
            Debug.Log($"CurrentHealth {_health.Value} health");
        }
    }
}