using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class AttackCancelSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _inAttackProcess;
        private ReactiveEvent _attackCanceledEvent;

        private CompositeCondition _mustCancelAttack;

        public void OnInit(Entity entity)
        {
            _inAttackProcess = entity.Get<InAttackProcess>();
            _attackCanceledEvent = entity.Get<AttackCancelEvent>();
            _mustCancelAttack = entity.Get<MustCancelAttack>();
        }

        public void OnUpdate(float deltaTime)
        {
            if(_inAttackProcess.Value == false) 
                return;
            
            if (_mustCancelAttack.Evaluate())
            {
                Debug.Log($"Attack canceled");
                _inAttackProcess.Value = false;
                _attackCanceledEvent?.Invoke();
            }
        }
    }
}