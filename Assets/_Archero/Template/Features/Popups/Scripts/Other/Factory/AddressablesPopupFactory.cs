using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RivalDom.Scripts.Domain.AddressableLoading;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;

namespace Template.Features.Popups
{
    public class AddressablesPopupFactory : PopupFactoryBase
    {
        private sealed class LoadedAsset
        {
            public readonly string Key;

            public UniTask LoadTask;
            public AddressableLoadResult Result;
            public bool IsLoaded;

            public int Users;
            public bool Pinned;

            public LoadedAsset(string key)
            {
                Key = key;
            }
        }
        
        private readonly Dictionary<string, LoadedAsset> _assets = new();

        public AddressablesPopupFactory(IClassFactory classFactory, IUILayersService layersService)
            : base(classFactory, layersService)
        {
        }

        protected override async UniTask<GameObject> AcquirePrefab(PopupEntryBase entry)
        {
            AddressablePopupEntry addressable = AsAddressable(entry);

            LoadedAsset asset = await AcquireAsset(addressable, addressable.KeepLoaded);
            return asset.Result.Prefab;
        }

        protected override void ReleasePrefab(PopupEntryBase entry)
        {
            AddressablePopupEntry addressable = AsAddressable(entry);

            if (_assets.TryGetValue(addressable.Prefab.AssetGUID, out LoadedAsset asset))
                ReleaseUser(asset);
        }

        public override async UniTask Preload(PopupEntryBase entry)
        {
            AddressablePopupEntry addressable = AsAddressable(entry);
            EnsureSource(addressable);

            LoadedAsset asset = await AcquireAsset(addressable, pin: true);

            asset.Users--;
        }

        public override void Unload(PopupEntryBase entry)
        {
            base.Unload(entry);

            AddressablePopupEntry addressable = entry as AddressablePopupEntry;
            if (addressable == null || !addressable.HasSource)
                return;

            if (_assets.TryGetValue(addressable.Prefab.AssetGUID, out LoadedAsset asset))
            {
                asset.Pinned = false;
                TryRelease(asset);
            }
        }
        
        private async UniTask<LoadedAsset> AcquireAsset(AddressablePopupEntry entry, bool pin)
        {
            string key = entry.Prefab.AssetGUID;

            if (!_assets.TryGetValue(key, out LoadedAsset asset))
            {
                asset = new LoadedAsset(key);
                asset.LoadTask = LoadInto(asset).Preserve();
                _assets[key] = asset;
            }

            asset.Users++;
            if (pin)
                asset.Pinned = true;

            try
            {
                await asset.LoadTask;
                return asset;
            }
            catch
            {
                asset.Users--;
                Discard(asset);
                throw;
            }
        }

        private static async UniTask LoadInto(LoadedAsset asset)
        {
            asset.Result = await AddressableObjectLoader.LoadAsset(asset.Key);
            asset.IsLoaded = true;
        }

        private void ReleaseUser(LoadedAsset asset)
        {
            asset.Users--;
            TryRelease(asset);
        }

        private void TryRelease(LoadedAsset asset)
        {
            if (asset.Users > 0 || asset.Pinned)
                return;

            Discard(asset);
        }

        private void Discard(LoadedAsset asset)
        {
            if (_assets.TryGetValue(asset.Key, out LoadedAsset current) && ReferenceEquals(current, asset))
                _assets.Remove(asset.Key);

            if (asset.IsLoaded)
            {
                asset.IsLoaded = false;
                AddressableObjectLoader.Release(asset.Result);
            }
        }

        private static AddressablePopupEntry AsAddressable(PopupEntryBase entry)
        {
            return entry as AddressablePopupEntry ??
                   throw new NotSupportedException(
                       $"{nameof(AddressablesPopupFactory)} supports only {nameof(AddressablePopupEntry)}, " +
                       $"got {(entry == null ? "null" : entry.GetType().Name)}.");
        }
    }
}