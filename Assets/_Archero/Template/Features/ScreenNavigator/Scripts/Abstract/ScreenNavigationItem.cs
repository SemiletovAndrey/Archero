using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Template.Features.Config.Local;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class ScreenNavigationItem<TPayload> : BaseNavigationItem<TPayload>
        where TPayload : BaseNavigationItemPayload, new()
    {
        private IScreenFactory _screenFactory;
        private ILocalConfigService _configService;

        private ScreenEntryBase _entry;
        private bool _entryResolved;

        private ActivatableScreen<TPayload> _screen;
        private bool _screenActivated;

        [Inject]
        private void Construct(IScreenFactory screenFactory, ILocalConfigService configService)
        {
            _screenFactory = screenFactory;
            _configService = configService;
        }

        protected ActivatableScreen<TPayload> Screen => _screen;

        protected virtual ScreenType ResolveScreenType()
        {
            ScreenTypeAttribute attribute = GetType().GetCustomAttribute<ScreenTypeAttribute>(inherit: false);
            return attribute != null ? attribute.Type : ScreenType.None;
        }

        public override async UniTask Activate(TPayload data)
        {
            if (await EnsureScreen())
            {
                _screenActivated = true;
                await _screen.Activate(data);
            }

            await base.Activate(data);
        }

        protected override async UniTask OnOpen(NavigationItemOpenPayload payload)
        {
            if (!await EnsureScreen())
                return;

            if (!_screenActivated)
            {
                _screenActivated = true;
                await _screen.Activate(new TPayload());
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

            ScreenConfig config = _configService.GetConfig<ScreenConfig>();
            if (!config.TryGetEntry(screenType, out ScreenEntryBase entry))
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