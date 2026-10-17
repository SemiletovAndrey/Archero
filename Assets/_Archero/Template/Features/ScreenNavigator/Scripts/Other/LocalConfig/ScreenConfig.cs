using System;
using System.Collections.Generic;
using Template.Features.Config.Local;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    [CreateAssetMenu(fileName = "Config_Screen_Container", menuName = "SO/Template/ScreenNavigator/ScreenConfig")]
    public class ScreenConfig : BaseLocalFeatureConfig
    {
        [field: SerializeField] public List<ScreenEntry> Entries = new();

        /// <summary>Экраны, загружаемые через Addressables.</summary>
        [field: SerializeField] public List<AddressableScreenEntry> AddressableEntries = new();

        [NonSerialized] private Dictionary<ScreenType, ScreenEntryBase> _index;

        /// <summary>Находит запись по типу экрана в обоих списках. Индекс строится один раз.</summary>
        public bool TryGetEntry(ScreenType type, out ScreenEntryBase entry)
        {
            _index ??= BuildIndex();
            return _index.TryGetValue(type, out entry);
        }

        private Dictionary<ScreenType, ScreenEntryBase> BuildIndex()
        {
            Dictionary<ScreenType, ScreenEntryBase> index = new();

            foreach (ScreenEntry entry in Entries)
                Register(index, entry);

            foreach (AddressableScreenEntry entry in AddressableEntries)
                Register(index, entry);

            return index;
        }

        private void Register(Dictionary<ScreenType, ScreenEntryBase> index, ScreenEntryBase entry)
        {
            if (entry == null)
                return;

            if (entry.Type == ScreenType.None)
            {
                Debug.LogError($"[{nameof(ScreenConfig)}] Config has an entry with ScreenType.None.", this);
                return;
            }

            if (!entry.HasSource)
            {
                Debug.LogError($"[{nameof(ScreenConfig)}] ScreenType.{entry.Type} doesn't have a prefab assigned.",
                    this);
                return;
            }

            if (!index.TryAdd(entry.Type, entry))
                Debug.LogError(
                    $"[{nameof(ScreenConfig)}] ScreenType.{entry.Type} appears several times in the configuration.",
                    this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Правка конфига в редакторе: индекс нужно построить заново.
            _index = null;
        }
#endif
    }
}