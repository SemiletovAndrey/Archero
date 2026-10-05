using System.Collections.Generic;
using Template.Features.Config.Local;
using UnityEngine;

namespace Template.Features.Popups
{
    [CreateAssetMenu(fileName = "Config_Popup_Settings", menuName = "SO/Template/Popup/Container")]
    public class PopupConfig : BaseLocalFeatureConfig
    {
        [field: SerializeField] public List<PopupEntry> Entries = new();
    }
}