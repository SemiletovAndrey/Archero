using UnityEngine;

namespace Template.Features.Config.Remote
{
    public abstract class BaseRemoteConfigProvider : ScriptableObject, IRemoteConfigProvider
    {
        public abstract int Priority { get; }
        public abstract bool TryGetConfig<TConfig>(string key, out TConfig config) where TConfig : class, IRemoteFeatureConfig;

        public virtual void Init()
        {
            
        }
    }
}