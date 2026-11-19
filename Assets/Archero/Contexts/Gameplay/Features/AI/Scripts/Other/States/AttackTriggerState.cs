using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.EntityCore.Features.Attack;
using Gameplay.Features.GameplayStateMachine;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.AI
{
    public class AttackTriggerState : State, IUpdatableState
    {
        private ReactiveEvent _attackRequest;

        public AttackTriggerState(Entity entity)
        {
            _attackRequest = entity.Get<StartAttackRequest>();
        }

        public override void Enter()
        {
            base.Enter();
            
            _attackRequest.Invoke();
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}