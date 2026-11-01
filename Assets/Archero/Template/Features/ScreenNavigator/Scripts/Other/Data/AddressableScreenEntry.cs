using UnityEngine.AddressableAssets;

namespace Template.Features.ScreenNavigator
{
    [System.Serializable]
    public class AddressableScreenEntry : ScreenEntryBase
    {
        public AssetReferenceGameObject Prefab;

        public bool KeepLoaded = true;

        public override bool HasSource => Prefab != null && Prefab.RuntimeKeyIsValid();

    }
}