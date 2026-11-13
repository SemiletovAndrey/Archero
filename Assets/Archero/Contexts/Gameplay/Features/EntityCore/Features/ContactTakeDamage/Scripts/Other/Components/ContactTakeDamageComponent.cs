using Contexts.Scripts.Utilities.Reactive;

namespace Gameplay.Features.ContactTakeDamage
{
    public class BodyContactDamage : ReactiveVariable<float>
    {
        public BodyContactDamage() { }
        public BodyContactDamage(float value) : base(value) { }
    }
}