using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class AttackDelayEndTriggerSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveEvent _attackDelayEndEvent;
        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<float> _delay;
        private ReactiveVariable<float> _attackProcessCurrentTime;

        private bool _alreadyAttack;

        private IDisposable _timerDisposable;
        private IDisposable _startAttackDisposable;
        
        public void OnInit(Entity entity)
        {
            _attackDelayEndEvent = entity.Get<AttackDelayEndEvent>();
            _startAttackEvent = entity.Get<StartAttackEvent>();
            _delay = entity.Get<AttackDelayTime>();
            _attackProcessCurrentTime = entity.Get<AttackProcessCurrentTime>();

            _timerDisposable = _attackProcessCurrentTime.Subscribe(OnTimerChanged);
            _startAttackDisposable = _startAttackEvent.Subscribe(OnStartAttack);
        }
        
        public void OnDispose()
        {
            _timerDisposable?.Dispose();
            _startAttackDisposable?.Dispose();
        }

        private void OnTimerChanged(float arg1, float timerValue)
        {
            if(_alreadyAttack)
                return;
            
            if (timerValue > _delay.Value)
            {
                _attackDelayEndEvent?.Invoke();
                _alreadyAttack = true;
            }
        }

        private void OnStartAttack()
        {
            _alreadyAttack = false;
        }       
    }
}