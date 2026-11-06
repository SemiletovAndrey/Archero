using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain.AddressableLoading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Template.Features.PrefabLoader
{
    public class PrefabProvider : IPrefabLoader, IDisposable
    {
        private readonly AddressablePrefabCache _cache = new();

        public UniTask Preload(IEnumerable<AssetReferenceGameObject> references, CancellationToken ct = default)
        {
            var tasks = new List<UniTask>();
            foreach (AssetReferenceGameObject reference in references)
                tasks.Add(_cache.Preload(GetKey(reference), ct));

            return UniTask.WhenAll(tasks);
        }

        public async UniTask<T> Acquire<T>(AssetReferenceGameObject reference, CancellationToken ct = default)
            where T : Component
        {
            string key = GetKey(reference);

            GameObject prefab = await _cache.Acquire(key, pin: true, ct);
            
            _cache.Release(key);

            return GetComponent<T>(prefab);
        }

        public bool TryGet<T>(AssetReferenceGameObject reference, out T prefab) where T : Component
        {
            if (_cache.TryGet(GetKey(reference), out GameObject go))
            {
                prefab = GetComponent<T>(go);
                return true;
            }

            prefab = null;
            return false;
        }

        public void Dispose()
        {
            _cache.Dispose();
        }

        private static string GetKey(AssetReferenceGameObject reference)
        {
            if (reference == null || !reference.RuntimeKeyIsValid())
                throw new ArgumentException("Addressable reference is not assigned.");

            return reference.AssetGUID;
        }

        private static T GetComponent<T>(GameObject prefab) where T : Component
        {
            if (!prefab.TryGetComponent(out T component))
                throw new InvalidOperationException($"Prefab '{prefab.name}' has no {typeof(T).Name}.");

            return component;
        }
    }
}