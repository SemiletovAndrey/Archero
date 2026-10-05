using UnityEngine;

namespace Template.Features.Config.Local
{
    public interface ILocalConfigProvider
    {
        public bool TryGet<TConfig>(out TConfig config) where TConfig : ILocalFeatureConfig;
    }
}