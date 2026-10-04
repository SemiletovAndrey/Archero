using System.Collections.Generic;
using UnityEngine;

namespace Template.Features.Config.Local
{
    [CreateAssetMenu(fileName = "Config_Local_Provider_Context", menuName = "SO/Template/Config/Local/ContextProvider")]
    public class ContextLocalConfigProvider : BaseLocalObject, ILocalConfigProvider
    {
        [SerializeField] private List<BaseLocalObject> _configs;

        public bool TryGet<TConfig>(out TConfig config) where TConfig : ILocalFeatureConfig
        {
            foreach (BaseLocalObject item in _configs)
            {
                switch (item)
                {
                    case ILocalConfigProvider configSource when configSource.TryGet(out config):
                        return true;
                    case TConfig item1:
                        config = item1;
                        return true;
                }
            }

            config = default;
            return false;
        }
    }
}