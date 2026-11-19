using System.Collections.Generic;
using Persistance.Features.EntityCore;

namespace Gameplay.Features.AI
{
    public interface ITargetSelector
    {
        public Entity SelectTargetFrom(IEnumerable<Entity> targets);
    }
}