namespace Persistance.Features.EntityCore
{
    public interface IInitializableSystem : IEntitySystem
    {
        public void OnInit(Entity entity);
    }
}