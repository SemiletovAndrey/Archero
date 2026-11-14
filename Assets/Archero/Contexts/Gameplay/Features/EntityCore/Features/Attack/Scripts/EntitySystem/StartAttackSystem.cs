using System;
using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class StartAttackSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveEvent _startAttackRequest;
        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<bool> _inAttackProcess;
        private CompositeCondition _canStartAttack;

        private IDisposable _attackRequestDispose;

        public void OnInit(Entity entity)
        {
            _startAttackRequest = entity.Get<StartAttackRequest>();
            _startAttackEvent = entity.Get<StartAttackEvent>();
            _inAttackProcess = entity.Get<InAttackProcess>();
            _canStartAttack = entity.Get<CanStartAttack>();

            _attackRequestDispose = _startAttackRequest.Subscribe(OnAttackRequest);
        }

        public void OnDispose()
        {
            _attackRequestDispose?.Dispose();
        }
        
        private void OnAttackRequest()
        {
            if (_canStartAttack.Evaluate())
            {
                _inAttackProcess.Value = true;
                _startAttackEvent?.Invoke();
                Debug.Log($"Start Attack");
            }
            else
            {
                Debug.Log($"Can't Start Attack");
            }
        }
    }
}