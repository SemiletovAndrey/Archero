using System;
using Contexts.Scripts.Utilities.Reactive;
using Template.Scripts.Utils;

namespace Gameplay.Features.LifeCycle
{
    public class CurrentHealth : ReactiveVariable<float>
    {
        public CurrentHealth() { }
        public CurrentHealth(float value) : base(value) {}
    }
    
    public class MaxHealth : ReactiveVariable<float>
    {
        public MaxHealth() { }
        public MaxHealth(float value) : base(value) {}
    }
    
    public class IsDead : ReactiveVariable<bool>
    {
        public IsDead() { }
        public IsDead(bool value) : base(value) {}
    }
    
    public class DeathProcessInitialTime : ReactiveVariable<float>
    {
        public DeathProcessInitialTime() { }
        public DeathProcessInitialTime(float value) : base(value) {}
    }
    
    public class DeathProcessCurrentTime : ReactiveVariable<float>
    {
        public DeathProcessCurrentTime() { }
        public DeathProcessCurrentTime(float value) : base(value) {}
    }
    
    public class InDeathProcess : ReactiveVariable<bool>
    {
        public InDeathProcess() { }
        public InDeathProcess(bool value) : base(value) {}  
    }
    
    public class MustDie : CompositeCondition
    {
        public MustDie() { }
        public MustDie(Func<bool, bool, bool> logicOperation) : base(logicOperation) { }
    }
    
    public class MustSelfRelease : CompositeCondition
    {
        public MustSelfRelease() { }
        public MustSelfRelease(Func<bool, bool, bool> logicOperation) : base(logicOperation) { }
    }
}