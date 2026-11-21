using Persistance.Features.EntityCore;

namespace Gameplay.Features.EntityCore
{
    public interface IEntityConfigService
    {
        public T Get<T>(EntityType type) where T : BaseEntityDataConfig;
    }
}