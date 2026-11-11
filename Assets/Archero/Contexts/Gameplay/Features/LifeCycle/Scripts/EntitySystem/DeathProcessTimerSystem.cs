using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using UnityEngine;

namespace Gameplay.Features.LifeCycle
{
    public class DeathProcessTimerSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _isDead;
        private ReactiveVariable<bool> _inDeathProcess;
        private ReactiveVariable<float> _initialTimer;
        private ReactiveVariable<float> _currentTime;

        private IDisposable _isDeadChangedDisposable;

        public void OnInit(Entity entity)
        {
            _isDead = entity.Get<IsDead>();
            _inDeathProcess =  entity.Get<InDeathProcess>();
            _initialTimer = entity.Get<DeathProcessInitialTime>();
            _currentTime = entity.Get<DeathProcessCurrentTime>();

            _isDeadChangedDisposable = _isDead.Subscribe(OnIsDeadChanged);
        }
        
        public void OnDispose()
        {
            _isDeadChangedDisposable?.Dispose();
        }
        
        public void OnUpdate(float deltaTime)
        {
            if(_inDeathProcess.Value == false)
                return;
            
            _currentTime.Value -= deltaTime;
            
            if(CooldownIsOver())
                _inDeathProcess.Value = false;
        }

        private void OnIsDeadChanged(bool arg1, bool isDead)
        {
            if (isDead)
            {
                _currentTime.Value = _initialTimer.Value;
                _inDeathProcess.Value = true;
            }
        }
        
        private bool CooldownIsOver()
        {
            return _currentTime.Value <= 0;
        }
    }
}