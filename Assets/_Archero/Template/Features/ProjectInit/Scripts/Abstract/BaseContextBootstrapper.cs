using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.ScreenNavigator;
using Zenject;

namespace Template.Features.ProjectInit
{
    public class BaseContextBootstrapper<TScene, TPayload> : BaseBootstrapper, IScenePayloadProvider<TPayload>
        where TScene : SceneSelectorNavigationItem<TPayload>
        where TPayload : BaseNavigationItemPayload
    {
        private IClassFactory _classFactory;
        private TScene _scene;
        
        private TPayload _payload;
        
        [Inject]
        private void Construct(IClassFactory classFactory, TScene scene)
        {
            _classFactory = classFactory;
            _scene = scene;
        }

        private void Awake()
        {
            _scene.AfterSceneOpened += OnSceneOpened;
            _scene.AfterSceneInitialized += DoAfterInitialization;
        }
        
        private void OnDestroy()
        {
            _scene.AfterSceneOpened -= OnSceneOpened;
            _scene.AfterSceneInitialized -= DoAfterInitialization;
        }
        
        public TPayload GetPayload()
        {
            return _payload;
        }
        
        protected virtual void DoAfterInitialization(TPayload payload)
        {
        }

        private async UniTask OnSceneOpened(TPayload payload)
        {
            _payload = payload;
            _classFactory.BindSelfAsSingle<IScenePayloadProvider<TPayload>>(this);

            await StartInitialization();
        }
    }
    
    public class BaseContextBootstrapper<TScene> : BaseContextBootstrapper<TScene, EmptyNavigationItemPayload>
        where TScene : SceneSelectorNavigationItem<EmptyNavigationItemPayload>
    {
    }
}