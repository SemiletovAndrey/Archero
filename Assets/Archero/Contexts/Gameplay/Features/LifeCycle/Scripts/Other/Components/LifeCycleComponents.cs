using Contexts.Scripts.Utilities.Reactive;

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
}