using Cysharp.Threading.Tasks;
using Menu.Features.MainMenu.Scripts;
using Template.Features.Popups;
using Template.Features.Presenters;
using UnityEngine;

namespace Menu.Features.MainMenu
{
    public class MainMenuPresenter : IPresenter<MainMenuPayload>
    {
        private readonly IMainMenuSystem _menuSystem;
        private readonly IMainMenuView _view;
        private readonly IPopupSystem _popupSystem;

        public MainMenuPresenter(IMainMenuSystem menuSystem, IMainMenuView view, IPopupSystem popupSystem)
        {
            _menuSystem = menuSystem;
            _view = view;
            _popupSystem = popupSystem;
        }

        public UniTask Activate(MainMenuPayload payload)
        {
            Debug.Log("Activating menu");
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            
        }
    }
}