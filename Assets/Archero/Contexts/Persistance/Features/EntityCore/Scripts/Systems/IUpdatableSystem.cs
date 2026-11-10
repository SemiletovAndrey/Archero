namespace Persistance.Features.EntityCore
{
    public interface IUpdatableSystem : IEntitySystem
    {
        public void OnUpdate(float deltaTime);
    }
}
