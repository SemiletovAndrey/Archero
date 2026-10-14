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
        public static UniTask<AddressableLoadResult> LoadAsset(AssetLabelReference assetReference)
        {
            return LoadInternal(Addressables.LoadAssetAsync<GameObject>(assetReference),
                $"reference '{assetReference}'");
        }

        public static UniTask<AddressableLoadResult> LoadAsset(string path)
        {
            return LoadInternal(Addressables.LoadAssetAsync<GameObject>(path), $"path '{path}'");
        }

        public static void Release(AddressableLoadResult result)
        {
            if (result.Handle.IsValid())
                Addressables.Release(result.Handle);
        }

        private static async UniTask<AddressableLoadResult> LoadInternal(AsyncOperationHandle<GameObject> handle,
            string description)
        {
            try
            {
                await handle.ToUniTask();
            }
            catch
            {
                ReleaseHandle(handle);
                throw;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
                return new AddressableLoadResult(handle.Result, handle);
            
            ReleaseHandle(handle);
            throw new Exception($"Failed to load asset with {description}");
        }

        private static void ReleaseHandle(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

    }
}