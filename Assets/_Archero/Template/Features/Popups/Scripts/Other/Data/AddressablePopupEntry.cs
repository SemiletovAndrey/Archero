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

        // Заполняется в редакторе (см. PopupConfig). Нужна, чтобы узнать класс попапа, не загружая префаб:
        // Addressable-ссылка не дает доступа к компонентам до загрузки.
        [SerializeField] private string _popupClassName;

        public override bool HasSource => Prefab != null && Prefab.RuntimeKeyIsValid();

        public override System.Type TryGetPopupClass()
        {
            GameObject prefab = Prefab != null ? Prefab.editorAsset as GameObject : null;
            if (prefab == null)
                return null;

            BasePopup popup = prefab.GetComponent<BasePopup>();
            return popup != null ? popup.GetType() : null;
        }

    }
}