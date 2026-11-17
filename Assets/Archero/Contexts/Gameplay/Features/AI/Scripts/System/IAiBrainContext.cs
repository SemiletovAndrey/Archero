using Persistance.Features.EntityCore;

namespace Gameplay.Features.AI
{
    public interface IAiBrainContext
    {
        public void SetFor(Entity entity, IBrain brain);
        public void Update(float deltaTime);
    }
}