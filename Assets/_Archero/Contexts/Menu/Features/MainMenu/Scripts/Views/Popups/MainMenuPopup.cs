using Cysharp.Threading.Tasks;
using Menu.Features.MainMenu.Scripts;
using Template.Features.Popups;
using Template.Features.UIExtensions;
using UnityEngine;
using Zenject;

namespace Menu.Features.MainMenu
{
    [PopupType(PopupType.MainMenu)]
    public class MainMenuPopup : BasePopupT<MainMenuPopupPayload>
    {
        [SerializeField] private UIButton _testButton;
        [SerializeField] private UIButton _closeButton;
        
        private IMainMenuSystem _menuSystem;
        public override UniTask Activate(MainMenuPopupPayload payload)
        {
            _menuSystem = payload.MainMenu;
            
            _testButton.AssignAction(TestSystems);
            _closeButton.AssignAction(ClosePopup);
            return UniTask.CompletedTask;
        }

        private void ClosePopup()
        {
            CloseExternal();
        }

        private void TestSystems()
        {
            _menuSystem.Test();
        }

        public override UniTask Deactivate()
        {
            return UniTask.CompletedTask;
        }

        public override UniTask Show()
        {
            return UniTask.CompletedTask;
        }

        public override UniTask Hide()
        {
            return UniTask.CompletedTask;
        }
    }
}