using UnityEngine;

namespace Template.Features.Config.Local
{
    public class SceneLocalConfigProvider //: BaseInitializableBehaviour, ILocalConfigProvider
    {
        /*[SerializeField] private ContextLocalConfigProvider _context;
        private ILocalConfigService _localConfigService;
        
        [Inject]
        private void Construct(ILocalConfigService localConfigService)
        {
            _localConfigService = localConfigService;
        }
        
        public override void Init()
        {
            if (_context == null)
            {
                return;
            }
            
            _localConfigService.AddProvider(this);
        }

        private void OnDestroy()
        {
            if (_context == null)
            {
                return;
            }
            
            _localConfigService.RemoveProvider(this);
        }

        public bool TryGet<TConfig>(out TConfig config) where TConfig : ILocalFeatureConfig
        {
            return _context.TryGet(out config);
        }*/
    }
}