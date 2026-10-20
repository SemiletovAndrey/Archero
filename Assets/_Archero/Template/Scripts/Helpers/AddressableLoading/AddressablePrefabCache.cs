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

            // Preserve позволяет нескольким вызывающим ждать одну и ту же загрузку.
            public UniTask LoadTask;
            public AddressableLoadResult Result;
            public bool IsLoaded;

            // Запись уже освобождена: если загрузка завершится позже, результат нужно сразу отпустить.
            public bool Discarded;

            // Число держателей (экземпляры, текущие загрузки). Закрепление (Pinned) учитывается отдельно.
            public int Users;
            public bool Pinned;

            public CacheEntry(string key)
            {
                Key = key;
            }
        }

        private readonly Dictionary<string, CacheEntry> _entries = new();

        /// <summary>Возвращает префаб и берет на него использование, которое держится до Release.</summary>
        public async UniTask<GameObject> Acquire(string key, bool pin)
        {
            CacheEntry entry = await AcquireInternal(key, pin);
            return entry.Result.Prefab;
        }

        /// <summary>Снимает использование, взятое в Acquire.</summary>
        public void Release(string key)
        {
            if (!_entries.TryGetValue(key, out CacheEntry entry))
                return;

            entry.Users--;
            TryDiscard(entry);
        }

        /// <summary>Загружает префаб и закрепляет его в памяти до Unpin.</summary>
        public async UniTask Preload(string key)
        {
            CacheEntry entry = await AcquireInternal(key, pin: true);

            // Preload'у собственная ссылка не нужна: ассет удерживается через Pinned.
            entry.Users--;
        }

        /// <summary>Снимает закрепление: ассет выгрузится, когда не останется использований.</summary>
        public void Unpin(string key)
        {
            if (!_entries.TryGetValue(key, out CacheEntry entry))
                return;

            entry.Pinned = false;
            TryDiscard(entry);
        }

        /// <summary>
        /// Освобождает все ассеты, включая закрепленные (кэш уничтожается, например, вместе со сценой).
        /// Загрузки, которые еще идут, будут освобождены сразу после завершения.
        /// </summary>
        public void ReleaseAll()
        {
            foreach (CacheEntry entry in new List<CacheEntry>(_entries.Values))
                Discard(entry);

            _entries.Clear();
        }

        /// <summary>
        /// Users и Pinned выставляются до первого await, поэтому Unpin или Release другого держателя
        /// не освободят ассет, пока мы ждем загрузку.
        /// </summary>
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
                // Упавшая загрузка не должна оставаться в кэше - следующая попытка загрузит ассет заново.
                entry.Users--;
                Discard(entry);
                throw;
            }
        }

        private static async UniTask LoadInto(CacheEntry entry)
        {
            entry.Result = await AddressableObjectLoader.LoadAsset(entry.Key);
            entry.IsLoaded = true;

            // Запись освободили, пока шла загрузка: отпускаем результат сразу, иначе ассет утечет.
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

            // Не трогаем запись, если под этим ключом уже лежит новая загрузка.
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