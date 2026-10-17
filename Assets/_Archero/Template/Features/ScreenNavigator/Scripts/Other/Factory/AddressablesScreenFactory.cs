using Cysharp.Threading.Tasks;
using Domain.AddressableLoading;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public class AddressablesScreenFactory : ScreenFactoryBase
    {
        private readonly AddressablePrefabCache _prefabs = new();

        public AddressablesScreenFactory(IClassFactory classFactory, IUILayersService layersService)
            : base(classFactory, layersService)
        {
        }

        protected override UniTask<GameObject> AcquirePrefab(ScreenEntryBase entry)
        {
            AddressableScreenEntry addressable = AsAddressable(entry);
            return _prefabs.Acquire(addressable.Prefab.AssetGUID, addressable.KeepLoaded);
        }

        protected override void ReleasePrefab(ScreenEntryBase entry)
        {
            _prefabs.Release(AsAddressable(entry).Prefab.AssetGUID);
        }

        public override UniTask Preload(ScreenEntryBase entry)
        {
            AddressableScreenEntry addressable = AsAddressable(entry);
            EnsureSource(addressable);

            return _prefabs.Preload(addressable.Prefab.AssetGUID);
        }

        public override void Unload(ScreenEntryBase entry)
        {
            // Сначала уничтожаем спрятанный в пуле экземпляр (он держит использование ассета).
            base.Unload(entry);

            AddressableScreenEntry addressable = entry as AddressableScreenEntry;
            if (addressable == null || !addressable.HasSource)
                return;

            _prefabs.Unpin(addressable.Prefab.AssetGUID);
        }

        private static AddressableScreenEntry AsAddressable(ScreenEntryBase entry)
        {
            return entry as AddressableScreenEntry ??
                   throw new System.NotSupportedException(
                       $"{nameof(AddressablesScreenFactory)} supports only {nameof(AddressableScreenEntry)}, " +
                       $"got {(entry == null ? "null" : entry.GetType().Name)}.");
        }
    }
}