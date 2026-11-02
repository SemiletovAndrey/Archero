using System.Collections.Generic;
using Template.Features.Config.Local;
using UnityEngine;

namespace Persistance.Features.LevelProgression
{
    [CreateAssetMenu(fileName = "LevelsListConfig", menuName = "SO/Persistence/LevelProgression/LevelsListConfig")]
    public class LevelsListConfig : BaseLocalFeatureConfig
    {
        [field: SerializeField] public List<LevelConfig> LevelConfigs { get; private set; }

        public LevelConfig GetBy(int level)
        {
            int levelIndex = level - 1;

            return LevelConfigs[levelIndex];
        }
    }
}