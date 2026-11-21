using Persistance.Features.EntityCore;
using Template.Features.Config.Local;

namespace Gameplay.Features.EntityCore
{
    public class EntityConfigService: IEntityConfigService
    {
        private readonly EntityDataConfig _config;

        public EntityConfigService(ILocalConfigService localConfigService)
        {
            _config = localConfigService.GetConfig<EntityDataConfig>();
        }

        public T Get<T>(EntityType type) where T : BaseEntityDataConfig
            => _config.GetConfig<T>(type);
    }
}