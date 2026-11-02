using System;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.Config.Local;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class ScreenNavigationItem<TPayload> : BaseNavigationItem<TPayload>
        where TPayload : BaseNavigationItemPayload
    {
        private enum ScreenState
        {
            None,
            Created,
            Prepared,
            Activated
        }
        private IScreenFactory _screenFactory;
        private ILocalConfigService _configService;
        
        private ScreenEntryBase _entry;
        private bool _entryResolved;

        private ActivatableScreen<TPayload> _screen;
        private ScreenState _state;

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

        protected virtual bool WaitFrameBeforeReveal => true;

        protected virtual ScreenType ResolveScreenType()
        {
            ScreenTypeAttribute attribute = GetType().GetCustomAttribute<ScreenTypeAttribute>(inherit: false);
            return attribute != null ? attribute.Type : ScreenType.None;
        }

        public override async UniTask Activate(TPayload data)
        {
            _payload = data;

            if (_state == ScreenState.Prepared)
            {
                _state = ScreenState.Activated;
            }
            else if (await EnsureScreen(CancellationToken.None))
            {
                await ActivateScreen(data, ScreenState.Activated);
            }

            await base.Activate(data);
        }

        internal override async UniTask Prepare(BaseNavigationItemPayload data, bool isTarget,
            CancellationToken cancellationToken)
        {
            if (isTarget)
            {
                if (data is not TPayload typed)
                    throw new InvalidCastException(
                        $"{GetType().Name} expects payload {typeof(TPayload).Name}, " +
                        $"got {(data == null ? "null" : data.GetType().Name)}.");

                _payload = typed;
            }

            if (!await EnsureScreen(cancellationToken))
                return;

            if (_state != ScreenState.Created || _payload == null)
                return;

            await ActivateScreen(_payload, ScreenState.Prepared);

            if (WaitFrameBeforeReveal)
                await UniTask.NextFrame(cancellationToken);
        }

        internal override void AbortPrepare()
        {
            if (_screen == null || IsActive)
                return;

            ActivatableScreen<TPayload> screen = _screen;
            _screen = null;
            _state = ScreenState.None;

            ReleaseScreen(screen);
        }

        internal override void ForceRelease()
        {
            ActivatableScreen<TPayload> screen = _screen;
            _screen = null;
            _state = ScreenState.None;
            IsActive = false;

            if (ReferenceEquals(screen, null))
                return;

            ReleaseScreen(screen);
        }

        protected override async UniTask OnOpen(NavigationItemOpenPayload payload)
        {
            if (!await EnsureScreen(CancellationToken.None))
                return;

            switch (_state)
            {
                case ScreenState.Created:
                    if (_payload == null)
                        throw new InvalidOperationException(
                            $"{GetType().Name} was opened before Activate: no payload to activate the screen with.");

                    await ActivateScreen(_payload, ScreenState.Activated);
                    break;

                case ScreenState.Prepared:
                    _state = ScreenState.Activated;
                    break;
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
            _state = ScreenState.None;

            try
            {
                await screen.Close(payload);
            }
            finally
            {
                ReleaseScreen(screen);
            }
        }

        private void ReleaseScreen(ActivatableScreen<TPayload> screen)
        {
            OnScreenReleasing(screen);

            if (screen != null)
                screen.SetVisible(false);

            _screenFactory.Release(screen);
        }

        private async UniTask<bool> EnsureScreen(CancellationToken cancellationToken)
        {
            if (_screen != null)
                return true;

            ScreenEntryBase entry = ResolveEntry();
            if (entry == null)
                return false;

            _screen = await _screenFactory.Create<ActivatableScreen<TPayload>>(
                entry, cancellationToken: cancellationToken);
            _state = ScreenState.Created;
            return true;
        }

        private async UniTask ActivateScreen(TPayload data, ScreenState resultState)
        {
            _state = resultState;

            await _screen.Activate(data);
            await OnScreenActivated(_screen, data);
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