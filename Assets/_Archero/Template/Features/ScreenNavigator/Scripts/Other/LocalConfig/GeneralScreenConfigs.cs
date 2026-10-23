using System.Collections.Generic;
using Template.Features.Config.Local;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    [CreateAssetMenu(fileName = "Config_GeneralScreens", menuName = "SO/Template/ScreenNavigator/GeneralContaienr")]
    public class GeneralScreenConfigs : BaseLocalFeatureConfig
    {
        [field: SerializeField] public List<ScreenConfig> ScreenConfigs;

        public bool TryGetScreenConfig(ScreenType screenType, out  ScreenEntryBase entry)
        {
            foreach (var config in ScreenConfigs)
            {
                if (config.TryGetEntry(screenType, out entry))
                {
                    return true;
                }
            }

            throw new KeyNotFoundException($"Screen Config {screenType} not found");
        }
    }
}