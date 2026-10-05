using System;
using UnityEngine;

namespace Template.Features.Popups
{
    [Serializable]
    public class PopupEntry
    {
        public PopupType Type;
        
        public GameObject Prefab;
    }
}