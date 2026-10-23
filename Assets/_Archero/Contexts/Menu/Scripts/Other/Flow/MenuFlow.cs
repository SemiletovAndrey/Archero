using System.Threading;
using Cysharp.Threading.Tasks;
using Menu.Features.MainMenu.Scripts;
using Template.Features.GameStateMachine;
using Template.Features.ScreenNavigator;
using UnityEngine;

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
                .NavigateTo<MainMenuItem, MainMenuPayload>(new MainMenuPayload(OnPlay))
                .AttachExternalCancellation(ct);

        private void OnPlay()
        {
            Debug.Log("OnPlay");
        }
    }
}