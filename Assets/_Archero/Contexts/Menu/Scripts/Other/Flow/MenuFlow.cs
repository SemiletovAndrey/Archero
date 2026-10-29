using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Features.Hud;
using Gameplay.Scripts;
using Menu.Features.MainMenu.Scripts;
using Template.Features.GameStateMachine;
using Template.Features.ScreenNavigator;

namespace Menu.Scripts
{
    public class MenuFlow
    {
        private readonly IScreenNavigator _screenNavigator;
        private readonly IGameStateMachine _stateMachine;

        public MenuFlow(IScreenNavigator screenNavigator, IGameStateMachine stateMachine)
        {
            _screenNavigator = screenNavigator;
            _stateMachine = stateMachine;
        }

        public UniTask ShowMainMenu(CancellationToken ct) =>
            _screenNavigator
                .NavigateTo<MainMenuItem, MainMenuScreenPayload>(new MainMenuScreenPayload(OnPlay))
                .AttachExternalCancellation(ct);

        private void OnPlay()
        {
            //_stateMachine.Enter<GameplayState>();

            _screenNavigator
                .NavigateTo<HudItem, HudScreenPayload>(new());
        }
    }
}