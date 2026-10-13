using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RivalDom.Scripts.Domain.AddressableLoading;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using Object = UnityEngine.Object;

namespace Template.Features.Popups
{
    public class AddressablesPopupFactory : PopupFactoryBase
    {
                private sealed class LoadedAsset
        {
            public readonly string Key;

            // Preserve позволяет нескольким вызывающим ждать одну и ту же загрузку.
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

        // Ключ - AssetGUID.
        private readonly Dictionary<string, LoadedAsset> _assets = new();

        // Какой ассет использует каждый созданный экземпляр.
        private readonly Dictionary<BasePopup, LoadedAsset> _instanceAssets = new();

        public AddressablesPopupFactory(IClassFactory classFactory, IUILayersService layersService)
            : base(classFactory, layersService)
        {
        }

        public override bool Supports(PopupEntryBase entry)
        {
            return entry is AddressablePopupEntry;
        }

        public override async UniTask<T> Create<T>(PopupEntryBase entry)
        {
            EnsureReady(entry);

            if (entry is not AddressablePopupEntry addressable)
                throw new NotSupportedException(
                    $"{nameof(AddressablesPopupFactory)} supports only {nameof(AddressablePopupEntry)}, got {entry.GetType().Name}.");

            LoadedAsset asset = await AcquireAsset(addressable, addressable.KeepLoaded);

            try
            {
                T popup = InstantiatePopup<T>(asset.Result.Prefab, entry);
                _instanceAssets[popup] = asset;
                return popup;
            }
            catch
            {
                ReleaseUser(asset);
                throw;
            }
        }

        public override void Destroy(BasePopup popup)
        {
            if (ReferenceEquals(popup, null))
                return;

            // Сначала уничтожаем экземпляр, потом отпускаем ассет.
            if (popup != null)
                Object.Destroy(popup.gameObject);

            if (_instanceAssets.TryGetValue(popup, out LoadedAsset asset))
            {
                _instanceAssets.Remove(popup);
                ReleaseUser(asset);
            }
        }

        public override async UniTask Preload(PopupEntryBase entry)
        {
            if (entry is not AddressablePopupEntry addressable)
                return;

            EnsureSource(addressable);

            LoadedAsset asset = await AcquireAsset(addressable, pin: true);

            // Preload'у собственная ссылка не нужна: ассет удерживается через Pinned.
            asset.Users--;
        }

        public override void Unload(PopupEntryBase entry)
        {
            if (entry is not AddressablePopupEntry addressable || !addressable.HasSource)
                return;

            if (_assets.TryGetValue(addressable.Prefab.AssetGUID, out LoadedAsset asset))
            {
                asset.Pinned = false;
                TryRelease(asset);
            }
        }

        /// <summary>
        /// Загружает ассет (одна загрузка на все параллельные запросы) и увеличивает Users.
        /// Users и Pinned выставляются до первого await, поэтому Unload или закрытие другого экземпляра
        /// не освободят ассет, пока мы ждем загрузку.
        /// </summary>
        private async UniTask<LoadedAsset> AcquireAsset(AddressablePopupEntry entry, bool pin)
        {
            // GUID ассета - валидный ключ Addressables, поэтому подходит перегрузка LoadAsset(string).
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
                // Упавшая загрузка не должна оставаться в кэше - следующая попытка загрузит ассет заново.
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

    }
}