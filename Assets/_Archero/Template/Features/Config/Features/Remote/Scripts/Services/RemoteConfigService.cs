using System.Collections.Generic;
using System.Linq;
using Template.Scripts.Utils;

namespace Template.Features.Config.Remote
{
    public class RemoteConfigService : IRemoteConfigService
    {
        private readonly List<IRemoteConfigProvider> _providers = new();

        public void Init()
        {
        }

        public TConfig GetConfig<TConfig>(string key) where TConfig : class, IRemoteFeatureConfig
        {
            if (TryGetConfig(key, out TConfig config))
            {
                return config;
            }

            throw new RemoteConfigNotFoundException(typeof(TConfig));
        }
        
        public bool TryGetConfig<TConfig>(string key, out TConfig config) where TConfig : class, IRemoteFeatureConfig
        {
            IEnumerable<IRemoteConfigProvider> providers = _providers;
            
            foreach (IRemoteConfigProvider provider in providers.Reverse())
            {
                if (provider.TryGetConfig(key, out config))
                {
                    return true;
                }
            }

            config = null;
            return false;
        }

        public void AddProvider(IRemoteConfigProvider provider)
        {
            _providers.Add(provider);
            _providers.Sort((provider1, provider2) => provider1.Priority.CompareTo(provider2.Priority));
        }

        public void RemoteProvider(IRemoteConfigProvider provider)
        {
            _providers.Remove(provider);
        }

        public TProvider GetProvider<TProvider>() where TProvider : IRemoteConfigProvider
        {
            return _providers.FirstOfType<TProvider>();
        }
    }
}