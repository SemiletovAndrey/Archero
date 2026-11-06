using System;
using System.Collections.Generic;
using System.Linq;
using Persistance.Features.EntityCore;
using Template.Features.Config.Local;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Template.Features.PrefabLoader
{
    [CreateAssetMenu(fileName = "Config_EntityContainer", menuName = "SO/Persistence/EntityConfig")]
    public class EntityConfig : BaseLocalFeatureConfig
    {
        [Serializable]
        public class Entry
        {
            public EntityType Type;
            public AssetReferenceGameObject Prefab;
        }

        [SerializeField] private List<Entry> _entries = new();

        private Dictionary<EntityType, AssetReferenceGameObject> _map;
        
        public IEnumerable<AssetReferenceGameObject> AllPrefabs => _entries.Select(e => e.Prefab);

        public AssetReferenceGameObject Get(EntityType type)
        {
            _map ??= _entries.ToDictionary(e => e.Type, e => e.Prefab);

            if (!_map.TryGetValue(type, out AssetReferenceGameObject reference))
                throw new KeyNotFoundException($"EntityConfig has no prefab for type '{type}'.");

            return reference;
        }

        private void OnValidate()
        {
            _map = null;

            var seen = new HashSet<EntityType>();
            foreach (Entry entry in _entries)
            {
                if (entry.Type == EntityType.None)
                    Debug.LogError($"{name}: entry with type None.", this);
                else if (!seen.Add(entry.Type))
                    Debug.LogError($"{name}: duplicate type '{entry.Type}'.", this);

                if (entry.Prefab == null || !entry.Prefab.RuntimeKeyIsValid())
                    Debug.LogError($"{name}: entry '{entry.Type}' has no prefab assigned.", this);
            }
        }
    }
}