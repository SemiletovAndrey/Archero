using System;
using System.Collections.Generic;
using System.Threading;
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

        public async UniTask<GameObject> Acquire(string key, bool pin, CancellationToken ct = default)
        {
            CacheEntry entry = await AcquireInternal(key, pin, ct);
            return entry.Result.Prefab;
        }

        public bool TryGet(string key, out GameObject prefab)
        {
            if (_entries.TryGetValue(key, out CacheEntry entry) && entry.IsLoaded && !entry.Discarded)
            {
                prefab = entry.Result.Prefab;
                return true;
            }

            prefab = null;
            return false;
        }

        public void Release(string key)
        {
            if (!_entries.TryGetValue(key, out CacheEntry entry))
                return;

            // Защита от Release без парного Acquire
            if (entry.Users > 0)
                entry.Users--;

            TryDiscard(entry);
        }

        public async UniTask Preload(string key, CancellationToken ct = default)
        {
            CacheEntry entry = await AcquireInternal(key, pin: true, ct);

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

        public void Dispose()
        {
            ReleaseAll();
        }

        private async UniTask<CacheEntry> AcquireInternal(string key, bool pin, CancellationToken ct)
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
                await entry.LoadTask.AttachExternalCancellation(ct);

                if (entry.Discarded)
                    throw new OperationCanceledException($"Prefab '{key}' was released while loading.");

                return entry;
            }
            catch
            {
                entry.Users--;

                if (entry.LoadTask.Status == UniTaskStatus.Faulted)
                    Discard(entry);
                else
                    TryDiscard(entry);

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