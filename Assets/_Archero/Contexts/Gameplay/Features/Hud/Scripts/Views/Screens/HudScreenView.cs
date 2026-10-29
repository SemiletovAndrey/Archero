using Cysharp.Threading.Tasks;
using Menu.Features.MainMenu.Scripts;
using Menu.Scripts;
using Template.Features.ScreenNavigator;
using Template.Features.UIExtensions;
using UnityEngine;
using Zenject;

namespace Gameplay.Features.Hud
{
    public class HudScreenView : ActivatableScreen<HudScreenPayload>, IHudView
    {
        private MenuFlow _menuFlow;
        [SerializeField] private UIButton _button;

        [Inject]
        private void Construct(MenuFlow menuFlow)
        {
            _menuFlow = menuFlow;
        }

        public override UniTask Activate(HudScreenPayload data)
        {
            _button.AssignAction(BackMenu);
            return UniTask.CompletedTask;
        }

        private void BackMenu()
        {
            _menuFlow.ShowMainMenu(destroyCancellationToken);
        }
    }
}