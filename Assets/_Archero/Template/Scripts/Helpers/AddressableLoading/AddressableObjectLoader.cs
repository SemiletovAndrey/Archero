using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace RivalDom.Scripts.Domain.AddressableLoading
{
    public static class AddressableObjectLoader
    {
        public static async UniTask<AddressableLoadResult> LoadAsset(AssetLabelReference assetReference)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(assetReference);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return new AddressableLoadResult(handle.Result, handle);
            }

            throw new Exception($"Failed to load asset with reference '{assetReference}'");
        }
        
        public static async UniTask<AddressableLoadResult> LoadAsset(string path)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(path);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return new AddressableLoadResult(handle.Result, handle);
            }

            throw new Exception($"Failed to load asset with path: '{path}'");
        }

        public static void Release(AddressableLoadResult result)
        {
            Addressables.Release(result.Handle);
        }
    }
}