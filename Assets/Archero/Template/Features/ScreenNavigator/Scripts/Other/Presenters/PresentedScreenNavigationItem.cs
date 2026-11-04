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
        where TPresenter : class, IScreenPresenter<TPayload>
    {
        private IPresenterFactory _presenterFactory;
        private IScreenPresenter<TPayload> _screenPresenter;

        [Inject]
        private void ConstructPresenter(IPresenterFactory presenterFactory)
        {
            _presenterFactory = presenterFactory;
        }

        protected override async UniTask OnScreenActivated(ActivatableScreen<TPayload> screen, TPayload data)
        {
            if (_screenPresenter == null)
            {
                if (screen is not TView view)
                    throw new InvalidOperationException(
                        $"{screen.GetType().Name} must implement {typeof(TView).Name}.");

                _screenPresenter = _presenterFactory.Create<TPresenter, TView>(view);
            }

            await _screenPresenter.Activate(data);
        }

        protected override void OnScreenReleasing(ActivatableScreen<TPayload> screen)
        {
            if (_screenPresenter == null)
                return;

            IScreenPresenter<TPayload> screenPresenter = _screenPresenter;
            _screenPresenter = null;

            try
            {
                screenPresenter.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

    }
}