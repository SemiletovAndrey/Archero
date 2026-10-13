using System;
using UnityEngine;

namespace Template.Features.Popups
{
    [Serializable]
    public class PopupEntry : PopupEntryBase
    {
        public GameObject Prefab;

        public override bool HasSource => Prefab != null;
        
        public override System.Type TryGetPopupClass()
        {
            if (Prefab == null)
                return null;

            BasePopup popup = Prefab.GetComponent<BasePopup>();
            return popup != null ? popup.GetType() : null;
        }

    }
}