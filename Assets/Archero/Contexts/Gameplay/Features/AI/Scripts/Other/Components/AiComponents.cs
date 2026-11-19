using Contexts.Scripts.Utilities.Reactive;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.AI
{
    public class CurrentTarget : ReactiveVariable<Entity>
    {
        public CurrentTarget() { }
        public CurrentTarget(Entity value) : base(value) { }
    }
}