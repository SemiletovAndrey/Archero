using System.Threading;
using Cysharp.Threading.Tasks;
using Menu.Features.MainMenu.Scripts;
using Template.Features.GameStateMachine;
using Template.Features.ScreenNavigator;

namespace Menu.Scripts.Init
{
    public class MenuSceneEntryPoint : ISceneEntryPoint
    {
        private readonly IScreenNavigator _screenNavigator;

        public MenuSceneEntryPoint(IScreenNavigator screenNavigator)
        {
            _screenNavigator = screenNavigator;
        }

        public async UniTask Enter(CancellationToken lifetime)
        {
            await _screenNavigator.NavigateTo<MainMenuItem, MainMenuPayload>(new MainMenuPayload()).AttachExternalCancellation(lifetime);
        }
    }
}