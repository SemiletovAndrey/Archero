using Cysharp.Threading.Tasks;
using Menu.Features.MainMenu.Scripts;
using Persistance.Features.Wallet;
using Template.Features.Popups;
using Template.Features.Presenters;

namespace Menu.Features.MainMenu
{
    public class MainMenuScreenPresenter : IScreenPresenter<MainMenuScreenPayload>
    {
        private readonly IMainMenuSystem _menuSystem;
        private readonly IMainMenuView _view;
        private readonly IPopupSystem _popupSystem;
        private readonly IPresenterFactory _presenterFactory;

        private MainMenuScreenPayload _payload;
        private WalletPresenter _presenter;

        public MainMenuScreenPresenter(IMainMenuSystem menuSystem, IMainMenuView view, IPopupSystem popupSystem,
            IPresenterFactory presenterFactory)
        {
            _menuSystem = menuSystem;
            _view = view;
            _popupSystem = popupSystem;
            _presenterFactory = presenterFactory;

            _presenter = _presenterFactory.Create<WalletPresenter, IWalletView>(view.WalletView);

            _view.PlayClicked += OnPlayClicked;
            _view.OpenPopupClicked += OnOpenPopupClicked;
        }

        public UniTask Activate(MainMenuScreenPayload screenPayload)
        {
            _payload = screenPayload;

            return UniTask.CompletedTask;
        }

        private void OnPlayClicked()
        {
            _payload?.OnPlay?.Invoke();
        }

        private void OnOpenPopupClicked()
        {
            _popupSystem.Open<MainMenuPopup, MainMenuPopupPayload>(new(_menuSystem));
        }

        public void Dispose()
        {
            _view.PlayClicked -= OnPlayClicked;
            _view.OpenPopupClicked -= OnOpenPopupClicked;
            
            _presenter?.Dispose();
        }
    }
}