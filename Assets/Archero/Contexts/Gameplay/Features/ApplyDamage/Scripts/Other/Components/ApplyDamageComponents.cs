using System;
using Contexts.Scripts.Utilities.Reactive;
using Template.Scripts.Utils;

namespace Gameplay.Features.ApplyDamage
{
    public class TakeDamageRequest : ReactiveEvent<float> { }
    public class TakeDamageEvent : ReactiveEvent<float> { }

    public class CanApplyDamage : CompositeCondition
    {
        public CanApplyDamage() { }
        public CanApplyDamage(Func<bool, bool, bool> logicOperation) : base(logicOperation) { }
    }
}