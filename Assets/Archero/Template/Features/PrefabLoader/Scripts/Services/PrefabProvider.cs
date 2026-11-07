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

        public async UniTask<T> Acquire<T>(AssetReferenceGameObject reference, CancellationToken ct = default)
            where T : Component
        {
            string key = GetKey(reference);

            GameObject prefab = await _cache.Acquire(key, pin: false, ct);

            if (!prefab.TryGetComponent(out T component))
            {
                _cache.Release(key);
                throw new InvalidOperationException($"Prefab '{prefab.name}' has no {typeof(T).Name}.");
            }

            return component;
        }

        public void Release(AssetReferenceGameObject reference)
        {
            _cache.Release(GetKey(reference));
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
    }
}