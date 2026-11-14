using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class AttackProcessTimerSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _currentTime;
        private ReactiveVariable<bool> _inAttackProcess;

        private ReactiveEvent _startAttackEvent;

        private IDisposable _startAttackEventDisposable;
        
        public void OnInit(Entity entity)
        {
            _currentTime = entity.Get<AttackProcessCurrentTime>();
            _inAttackProcess = entity.Get<InAttackProcess>();
            _startAttackEvent = entity.Get<StartAttackEvent>();

            _startAttackEventDisposable = _startAttackEvent.Subscribe(OnStartAttackProcess);
        }
        
        public void OnDispose()
        {
            _startAttackEventDisposable?.Dispose();
        }

        private void OnStartAttackProcess()
        {
            _currentTime.Value = 0;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_inAttackProcess.Value == false)
                return;
            
            _currentTime.Value += deltaTime;
        }
    }
}