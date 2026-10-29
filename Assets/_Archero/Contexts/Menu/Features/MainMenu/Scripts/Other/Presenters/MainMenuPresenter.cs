using Cysharp.Threading.Tasks;
using Menu.Features.MainMenu.Scripts;
using Template.Features.Popups;
using Template.Features.Presenters;
using UnityEngine;

namespace Menu.Features.MainMenu
{
    public class MainMenuPresenter : IPresenter<MainMenuScreenPayload>
    {
        private readonly IMainMenuSystem _menuSystem;
        private readonly IMainMenuView _view;
        private readonly IPopupSystem _popupSystem;
        
        private MainMenuScreenPayload _payload;

        public MainMenuPresenter(IMainMenuSystem menuSystem, IMainMenuView view, IPopupSystem popupSystem)
        {
            _menuSystem = menuSystem;
            _view = view;
            _popupSystem = popupSystem;
            
            _view.PlayClicked += OnPlayClicked;
        }

        public UniTask Activate(MainMenuScreenPayload screenPayload)
        {
            Debug.Log("Activating menu");
            _payload = screenPayload;
            
            _popupSystem.Open<MainMenuPopup, MainMenuPopupPayload>(new(_menuSystem)).Forget();
            return UniTask.CompletedTask;
        }

        private void OnPlayClicked()
        {
            _payload?.OnPlay?.Invoke();
        }

        public void Dispose()
        {
            _view.PlayClicked -= OnPlayClicked;
        }
    }
}