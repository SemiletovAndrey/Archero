using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Domain.AddressableLoading
{
    public class AddressablePrefabCache
    {
        private sealed class CacheEntry
        {
            public readonly string Key;

            public UniTask LoadTask;
            public AddressableLoadResult Result;
            public bool IsLoaded;

            public bool Discarded;

            public int Users;
            public bool Pinned;

            public CacheEntry(string key)
            {
                Key = key;
            }
        }

        private readonly Dictionary<string, CacheEntry> _entries = new();

        public async UniTask<GameObject> Acquire(string key, bool pin)
        {
            CacheEntry entry = await AcquireInternal(key, pin);
            return entry.Result.Prefab;
        }

        public void Release(string key)
        {
            if (!_entries.TryGetValue(key, out CacheEntry entry))
                return;

            entry.Users--;
            TryDiscard(entry);
        }

        public async UniTask Preload(string key)
        {
            CacheEntry entry = await AcquireInternal(key, pin: true);

            entry.Users--;
        }

        public void Unpin(string key)
        {
            if (!_entries.TryGetValue(key, out CacheEntry entry))
                return;

            entry.Pinned = false;
            TryDiscard(entry);
        }
        
        public void ReleaseAll()
        {
            foreach (CacheEntry entry in new List<CacheEntry>(_entries.Values))
                Discard(entry);

            _entries.Clear();
        }
        
        private async UniTask<CacheEntry> AcquireInternal(string key, bool pin)
        {
            if (!_entries.TryGetValue(key, out CacheEntry entry))
            {
                entry = new CacheEntry(key);
                entry.LoadTask = LoadInto(entry).Preserve();
                _entries[key] = entry;
            }

            entry.Users++;
            if (pin)
                entry.Pinned = true;

            try
            {
                await entry.LoadTask;
                return entry;
            }
            catch
            {
                entry.Users--;
                Discard(entry);
                throw;
            }
        }

        private static async UniTask LoadInto(CacheEntry entry)
        {
            entry.Result = await AddressableObjectLoader.LoadAsset(entry.Key);
            entry.IsLoaded = true;

            if (entry.Discarded)
            {
                entry.IsLoaded = false;
                AddressableObjectLoader.Release(entry.Result);
            }
        }

        private void TryDiscard(CacheEntry entry)
        {
            if (entry.Users > 0 || entry.Pinned)
                return;

            Discard(entry);
        }

        private void Discard(CacheEntry entry)
        {
            entry.Discarded = true;

            if (_entries.TryGetValue(entry.Key, out CacheEntry current) && ReferenceEquals(current, entry))
                _entries.Remove(entry.Key);

            if (entry.IsLoaded)
            {
                entry.IsLoaded = false;
                AddressableObjectLoader.Release(entry.Result);
            }
        }
    }
}