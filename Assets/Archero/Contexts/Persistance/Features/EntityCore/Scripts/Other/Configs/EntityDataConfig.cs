using System;
using System.Collections.Generic;
using System.Linq;
using Template.Features.Config.Local;
using UnityEngine;

namespace Persistance.Features.EntityCore
{
    [CreateAssetMenu(fileName = "Config_EntityDataContainer",
        menuName = "SO/Persistence/EntityDataConfigs")]
    public class EntityDataConfig : BaseLocalFeatureConfig
    {
        [Serializable]
        public class Entry
        {
            [SerializeField] private EntityType _type;
            [SerializeField] private BaseEntityDataConfig _config;

            public EntityType Type => _type;
            public BaseEntityDataConfig Config => _config;
        }

        [SerializeField] private List<Entry> _entries = new();

        public IReadOnlyList<Entry> Entries => _entries;
        
        public T GetConfig<T>(EntityType type)
            where T : BaseEntityDataConfig
        {
            BaseEntityDataConfig config = Entries
                .FirstOrDefault(x => x.Type == type)?.Config;

            if (config is not T result)
                throw new InvalidOperationException(
                    $"Entity config for {type} is missing or has invalid type.");

            return result;
        }
    }
}