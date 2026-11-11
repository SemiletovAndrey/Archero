using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.LifeCycle
{
    public class DeathSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<bool> _isDead;
        
        private ReactiveVariable<float> _currentHealth;

        private MustDie _mustDie;
        
        private IDisposable _healthChangeSubscription; 
        
        public void OnInit(Entity entity)
        {
            _isDead = entity.Get<IsDead>();
            _currentHealth = entity.Get<CurrentHealth>();
            _mustDie = entity.Get<MustDie>();

            _healthChangeSubscription = _currentHealth.Subscribe(OnHealthChanged);
        }
        
        public void OnDispose()
        {
            _healthChangeSubscription.Dispose();
        }

        private void OnHealthChanged(float oldValue, float newValue)
        {
            if(_isDead.Value)
                return;
            
            if (_mustDie.Evaluate())
            {
                _isDead.Value = true;
            }
        }
    }
}