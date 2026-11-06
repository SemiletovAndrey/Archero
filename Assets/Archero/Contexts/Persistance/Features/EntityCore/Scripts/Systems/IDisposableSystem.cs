namespace Persistance.Features.EntityCore
{
    public interface IDisposableSystem : IEntitySystem
    {
        public void OnDispose();
    }
}