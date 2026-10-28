using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Template.Features.Config.Local;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class ScreenNavigationItem<TPayload> : BaseNavigationItem<TPayload>
        where TPayload : BaseNavigationItemPayload
    {
        private IScreenFactory _screenFactory;
        private ILocalConfigService _configService;

        private ScreenEntryBase _entry;
        private bool _entryResolved;

        private ActivatableScreen<TPayload> _screen;
        private bool _screenActivated;
        
        private TPayload _payload;

        [Inject]
        private void Construct(IScreenFactory screenFactory, ILocalConfigService configService)
        {
            _screenFactory = screenFactory;
            _configService = configService;
        }

        protected ActivatableScreen<TPayload> Screen => _screen;
        
        protected virtual UniTask OnScreenActivated(ActivatableScreen<TPayload> screen, TPayload data)
            => UniTask.CompletedTask;

        protected virtual void OnScreenReleasing(ActivatableScreen<TPayload> screen) { }

        protected virtual ScreenType ResolveScreenType()
        {
            ScreenTypeAttribute attribute = GetType().GetCustomAttribute<ScreenTypeAttribute>(inherit: false);
            return attribute != null ? attribute.Type : ScreenType.None;
        }

        public override async UniTask Activate(TPayload data)
        {
            _payload = data;
            
            if (await EnsureScreen())
            {
                _screenActivated = true;
                await _screen.Activate(data);
                await OnScreenActivated(_screen, data);
            }

            await base.Activate(data);
        }

        protected override async UniTask OnOpen(NavigationItemOpenPayload payload)
        {
            if (!await EnsureScreen())
                return;

            if (!_screenActivated)
            {
                if (_payload == null)
                    throw new InvalidOperationException(
                        $"{GetType().Name} was opened before Activate: no payload to activate the screen with.");

                _screenActivated = true;
                await _screen.Activate(_payload);
                await OnScreenActivated(_screen, _payload);
            }

            _screen.SetVisible(true);
            await _screen.Open(payload);
        }

        protected override async UniTask OnClose(NavigationItemClosePayload payload)
        {
            if (_screen == null)
                return;

            ActivatableScreen<TPayload> screen = _screen;
            _screen = null;
            _screenActivated = false;

            try
            {
                await screen.Close(payload);
            }
            finally
            {
                OnScreenReleasing(screen); 
                screen.SetVisible(false);
                _screenFactory.Release(screen);
            }
        }

        private async UniTask<bool> EnsureScreen()
        {
            if (_screen != null)
                return true;

            ScreenEntryBase entry = ResolveEntry();
            if (entry == null)
                return false;

            _screen = await _screenFactory.Create<ActivatableScreen<TPayload>>(entry);
            return true;
        }

        private ScreenEntryBase ResolveEntry()
        {
            if (_entryResolved)
                return _entry;

            ScreenType screenType = ResolveScreenType();
            if (screenType == ScreenType.None)
            {
                _entryResolved = true;
                return null;
            }

            GeneralScreenConfigs config = _configService.GetConfig<GeneralScreenConfigs>();
            if (!config.TryGetScreenConfig(screenType, out ScreenEntryBase entry))
                throw new InvalidOperationException(
                    $"No screen configured for ScreenType.{screenType} (item {GetType().Name}).");

            _entry = entry;
            _entryResolved = true;
            return entry;
        }
    }

    public abstract class ScreenNavigationItem : ScreenNavigationItem<EmptyNavigationItemPayload>
    {
    }
}