using System.Collections.Generic;
using Template.Features.Config.Local;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    [CreateAssetMenu(fileName = "Config_Screen_Container", menuName = "SO/Template/ScreenNavigator/ScreenConfig")]
    public class ScreenConfig: BaseLocalFeatureConfig
    {
        [field: SerializeField] public List<ScreenEntry> Entries = new();
    }
}