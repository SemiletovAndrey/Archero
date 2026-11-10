namespace Persistance.Features.EntityCore
{
    public interface IFixedUpdatableSystem : IEntitySystem
    {
        public void OnFixedUpdate(float fixedDeltaTime);
    }
}
