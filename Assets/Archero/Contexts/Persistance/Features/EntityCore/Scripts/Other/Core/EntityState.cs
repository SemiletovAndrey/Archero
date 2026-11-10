namespace Persistance.Features.EntityCore
{
    public enum EntityState
    {
        Created = 0,
        Initializing = 1,
        Active = 2,
        Releasing = 3,
        Disposed = 4
    }
}
