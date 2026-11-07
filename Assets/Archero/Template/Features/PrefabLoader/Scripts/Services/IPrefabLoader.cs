using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Template.Features.PrefabLoader
{
    public interface IPrefabLoader
    {
        public UniTask<T> Acquire<T>(AssetReferenceGameObject reference, CancellationToken ct = default)
            where T : Component;
        
        public void Release(AssetReferenceGameObject reference);
    }
}