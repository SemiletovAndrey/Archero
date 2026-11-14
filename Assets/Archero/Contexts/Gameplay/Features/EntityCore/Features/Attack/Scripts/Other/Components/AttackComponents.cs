using System;
using Contexts.Scripts.Utilities.Reactive;
using Template.Scripts.Utils;

namespace Gameplay.Features.EntityCore.Features.Attack
{
    public class StartAttackRequest : ReactiveEvent { }

    public class StartAttackEvent : ReactiveEvent { }

    public class CanStartAttack : CompositeCondition
    {
        public CanStartAttack() { }
        public CanStartAttack(Func<bool, bool, bool> logicOperation) : base(logicOperation) { }
    }

    public class EndAttackEvent : ReactiveEvent { }

    public class AttackProcessInitialTime : ReactiveVariable<float>
    {
        public AttackProcessInitialTime() { }
        public AttackProcessInitialTime(float value) : base(value) { }
    }
    
    public class AttackProcessCurrentTime : ReactiveVariable<float>
    {
        public AttackProcessCurrentTime() { }
        public AttackProcessCurrentTime(float value) : base(value) { }
    }

    public class InAttackProcess : ReactiveVariable<bool>
    {
        public InAttackProcess() { }
        public InAttackProcess(bool value) : base(value) { }
    }

    public class AttackDelayTime : ReactiveVariable<float>
    {
        public AttackDelayTime() { }
        public AttackDelayTime(float value) : base(value) { }
    }
    
    public class AttackDelayEndEvent : ReactiveEvent { }

    public class InstantAttackDamage : ReactiveVariable<float>
    {
        public InstantAttackDamage() { }
        public InstantAttackDamage(float value) : base(value) { }
    }
}