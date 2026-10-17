using UnityEngine.AddressableAssets;

namespace Template.Features.ScreenNavigator
{
    public class AddressableScreenEntry : ScreenEntryBase
    {
        public AssetReferenceGameObject Prefab;

        /// <summary>
        /// true - после первой загрузки ассет остается в памяти.
        /// false - ассет выгружается, когда уничтожен последний экземпляр.
        /// </summary>
        public bool KeepLoaded = true;

        public override bool HasSource => Prefab != null && Prefab.RuntimeKeyIsValid();

    }
}