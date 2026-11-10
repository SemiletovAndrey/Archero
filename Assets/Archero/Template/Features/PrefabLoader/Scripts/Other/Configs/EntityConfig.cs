using System;
using System.Collections.Generic;
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
            [SerializeField] private EntityType _type;
            [SerializeField] private AssetReferenceGameObject _prefab;

            public EntityType Type => _type;
            public AssetReferenceGameObject Prefab => _prefab;
        }

        [SerializeField] private List<Entry> _entries = new();

        public IReadOnlyList<Entry> Entries => _entries;

        private void OnValidate()
        {
            var seen = new HashSet<EntityType>();
            foreach (Entry entry in _entries)
            {
                if (!seen.Add(entry.Type))
                    Debug.LogError($"{name}: duplicate type '{entry.Type}'.", this);

                if (entry.Prefab == null || !entry.Prefab.RuntimeKeyIsValid())
                    Debug.LogError($"{name}: entry '{entry.Type}' has no prefab assigned.", this);
            }
        }
    }
}