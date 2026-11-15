using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class AttackCooldownTimerSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _currentTime;
        private ReactiveVariable<float> _initialTime;
        private ReactiveVariable<bool> _inAttackCoolDown;

        private ReactiveEvent _endAttackEvent;

        private IDisposable _endAttackEventDisposable;
        
        public void OnInit(Entity entity)
        {
            _currentTime = entity.Get<AttackCooldownCurrentTime>();
            _initialTime = entity.Get<AttackCooldownInitialTime>();
            _inAttackCoolDown = entity.Get<InAttackCooldown>();
            _endAttackEvent = entity.Get<EndAttackEvent>();

            _endAttackEventDisposable = _endAttackEvent.Subscribe(OnEndAttackEvent);
        }
        
        public void OnUpdate(float deltaTime)
        {
            if (!_inAttackCoolDown.Value)
                return;
            
            _currentTime.Value -= deltaTime;

            if (ColdownIsOver())
            {
                _inAttackCoolDown.Value = false;
                Debug.Log($"Cooldown End");
            }
        }

        public void OnDispose()
        {
            _endAttackEventDisposable?.Dispose();
        }

        private void OnEndAttackEvent()
        {
            Debug.Log($"Cooldown Start");
            _currentTime.Value = _initialTime.Value;
            _inAttackCoolDown.Value = true;
        }
        
        private bool ColdownIsOver()
        {
            return _currentTime.Value <= 0;
        }
    }
}