using System;
using Cysharp.Threading.Tasks;
using Template.Features.Presenters;
using UnityEngine;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class PresentedScreenNavigationItem<TPayload, TView, TPresenter>
        : ScreenNavigationItem<TPayload>
        where TPayload : BaseNavigationItemPayload
        where TView : class
        where TPresenter : class, IPresenter<TPayload>
    {
        private IPresenterFactory _presenterFactory;
        private IPresenter<TPayload> _presenter;

        [Inject]
        private void ConstructPresenter(IPresenterFactory presenterFactory)
        {
            _presenterFactory = presenterFactory;
        }

        protected override async UniTask OnScreenActivated(ActivatableScreen<TPayload> screen, TPayload data)
        {
            if (_presenter == null)
            {
                if (screen is not TView view)
                    throw new InvalidOperationException(
                        $"{screen.GetType().Name} must implement {typeof(TView).Name}.");

                _presenter = _presenterFactory.Create<TPresenter, TView>(view);
            }

            await _presenter.Activate(data);
        }

        protected override void OnScreenReleasing(ActivatableScreen<TPayload> screen)
        {
            if (_presenter == null)
                return;

            IPresenter<TPayload> presenter = _presenter;
            _presenter = null;

            try
            {
                presenter.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

    }
}