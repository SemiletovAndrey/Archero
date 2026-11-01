using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain.AddressableLoading;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;

namespace Template.Features.Popups
{
    public class AddressablesPopupFactory : PopupFactoryBase
    {
        private readonly AddressablePrefabCache _prefabs = new();

        public AddressablesPopupFactory(IClassFactory classFactory, IUILayersService layersService)
            : base(classFactory, layersService)
        {
        }

        protected override UniTask<GameObject> AcquirePrefab(PopupEntryBase entry)
        {
            AddressablePopupEntry addressable = AsAddressable(entry);
            return _prefabs.Acquire(addressable.Prefab.AssetGUID, addressable.KeepLoaded);
        }

        protected override void ReleasePrefab(PopupEntryBase entry)
        {
            _prefabs.Release(AsAddressable(entry).Prefab.AssetGUID);
        }

        public override UniTask Preload(PopupEntryBase entry, CancellationToken cancellationToken = default)
        {
            AddressablePopupEntry addressable = AsAddressable(entry);
            EnsureSource(addressable);

            return _prefabs.Preload(addressable.Prefab.AssetGUID).AttachExternalCancellation(cancellationToken);
        }

        public override void Unload(PopupEntryBase entry)
        {
            base.Unload(entry);

            AddressablePopupEntry addressable = entry as AddressablePopupEntry;
            if (addressable == null || !addressable.HasSource)
                return;

            _prefabs.Unpin(addressable.Prefab.AssetGUID);
        }

        protected override void OnDispose()
        {
            _prefabs.ReleaseAll();
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