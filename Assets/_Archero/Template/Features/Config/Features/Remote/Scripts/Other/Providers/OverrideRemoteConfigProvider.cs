using Template.Features.ProjectInit;
using UnityEngine;
using Zenject;

namespace Template.Features.Config.Remote
{
    public class OverrideRemoteConfigProvider : BaseInitializableBehaviour, IRemoteConfigProvider
    {
        [SerializeField] private BaseRemoteConfigProvider _context;
        private IRemoteConfigService _remoteConfigService;

        [Inject]
        private void Construct(IRemoteConfigService remoteConfigService)
        {
            _remoteConfigService = remoteConfigService;
        }
        
        public int Priority => 999;
        
        public override void Init()
        {
            if (_context == null)
            {
                return;
            }
            
            _context.Init();
            _remoteConfigService.AddProvider(this);
        }

        private void OnDestroy()
        {
            if (_context == null)
            {
                return;
            }
            
            _remoteConfigService.RemoteProvider(this);
        }

        public bool TryGetConfig<TConfig>(string key, out TConfig config) where TConfig : class, IRemoteFeatureConfig
        {
            return _context.TryGetConfig(key, out config);
        }
    }
}