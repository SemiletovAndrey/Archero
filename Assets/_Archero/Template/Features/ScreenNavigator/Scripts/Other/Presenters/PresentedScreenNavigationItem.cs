using System;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class PresentedScreenNavigationItem<TPayload, TView, TPresenter>
        : ScreenNavigationItem<TPayload>
        where TPayload : BaseNavigationItemPayload
        where TView : class
        where TPresenter : class, IScreenPresenter<TPayload>
    {
        private IClassFactory _classFactory;
        private TPresenter _presenter;

        [Inject]
        private void Construct(IClassFactory classFactory)
        {
            _classFactory = classFactory;
        }

        protected override async UniTask OnScreenActivated(ActivatableScreen<TPayload> screen, TPayload data)
        {
            if (_presenter == null)
            {
                if (screen is not TView view)
                    throw new InvalidOperationException(
                        $"{screen.GetType().Name} must implement {typeof(TView).Name}.");

                _presenter = _classFactory.Create<TPresenter>(new object[] { view });
            }

            await _presenter.Activate(data);
        }

        protected override void OnScreenReleasing(ActivatableScreen<TPayload> screen)
        {
            _presenter?.Dispose();
            _presenter = null;
        }
    }
}