using System;
using System.Collections.Generic;
using UnityEngine;

namespace Template.Features.Config.Local
{
    public class LocalConfigService : ILocalConfigService
    {
        private List<ILocalConfigProvider> _sources = new();
        
        public void Init()
        {
        }

        public TConfig GetConfig<TConfig>() where TConfig : ILocalFeatureConfig
        {
            foreach (ILocalConfigProvider source in _sources)
            {
                if (source.TryGet(out TConfig config))
                {
                    return config;
                }
            }

            throw new LocalConfigNotFoundException(typeof(TConfig));
        }

        public void AddProvider(ILocalConfigProvider provider)
        {
            _sources.Add(provider);
        }
        
        public void RemoveProvider(ILocalConfigProvider provider)
        {
            
            _sources.Remove(provider);
        }
    }
}