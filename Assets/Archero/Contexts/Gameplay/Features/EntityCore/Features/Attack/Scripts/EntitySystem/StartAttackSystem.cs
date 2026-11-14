using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;
using Template.Scripts.Utils;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class StartAttackSystem : IInitializableSystem
    {
        private ReactiveEvent _startAttackRequest;
        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<bool> _inAttackProcess;
        private CompositeCondition _canStartAttack;


        public void OnInit(Entity entity)
        {
            _startAttackRequest = entity.Get<StartAttackRequest>();
            _startAttackEvent = entity.Get<StartAttackEvent>();
            _inAttackProcess = entity.Get<InAttackProcess>();
            _canStartAttack = entity.Get<CanStartAttack>();
        }
    }
}