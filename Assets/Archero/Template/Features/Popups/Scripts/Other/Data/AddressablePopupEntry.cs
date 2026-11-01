using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Template.Features.Popups
{
    [Serializable]
    public class AddressablePopupEntry : PopupEntryBase
    {
        public AssetReferenceGameObject Prefab;
        
        public bool KeepLoaded = true;

        public override bool HasSource => Prefab != null && Prefab.RuntimeKeyIsValid();

#if UNITY_EDITOR
        public override System.Type TryGetPopupClass()
        {
            GameObject prefab = Prefab != null ? Prefab.editorAsset as GameObject : null;
            if (prefab == null)
                return null;

            BasePopup popup = prefab.GetComponent<BasePopup>();
            return popup != null ? popup.GetType() : null;
        }
#endif
    }
}