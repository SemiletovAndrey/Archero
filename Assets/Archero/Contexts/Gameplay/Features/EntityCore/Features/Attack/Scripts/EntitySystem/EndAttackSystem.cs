using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class EndAttackSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<float> _currentTime;
        private ReactiveVariable<float> _initialTime;
        private ReactiveVariable<bool> _inAttackProcess;

        private ReactiveEvent _endAttackEvent;

        private IDisposable _timerDisposable;
        
        public void OnInit(Entity entity)
        {
            _currentTime = entity.Get<AttackProcessCurrentTime>();
            _initialTime = entity.Get<AttackProcessInitialTime>();
            _inAttackProcess =  entity.Get<InAttackProcess>();
            _endAttackEvent = entity.Get<EndAttackEvent>();

            _timerDisposable = _currentTime.Subscribe(OnTimerChanged);
        }
        
        public void OnDispose()
        {
            _timerDisposable?.Dispose();
        }

        private void OnTimerChanged(float arg1, float currentTime)
        {
            if (TimeIsDone(currentTime))
            {
                _inAttackProcess.Value = false;
                _endAttackEvent?.Invoke();
            }
        }

        private bool TimeIsDone(float currentTime)
        {
            return currentTime >= _initialTime.Value;
        }
    }
}