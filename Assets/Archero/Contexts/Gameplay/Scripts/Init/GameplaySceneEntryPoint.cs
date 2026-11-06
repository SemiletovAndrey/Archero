using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Features.Hud;
using Persistance.Features.GameLoopFeature;
using Template.Features.GameStateMachine;
using Template.Features.ScreenNavigator;

namespace Gameplay.Scripts
{
    public class GameplaySceneEntryPoint : ISceneEntryPoint
    {
        private readonly IScreenNavigator _screenNavigator;
        private readonly TestGameplay _testGameplay;
        private readonly GameplayLoopService _gameplayLoopService;

        public GameplaySceneEntryPoint(IScreenNavigator screenNavigator, TestGameplay testGameplay, GameplayLoopService gameplayLoopService)
        {
            _screenNavigator = screenNavigator;
            _testGameplay = testGameplay;
            _gameplayLoopService = gameplayLoopService;
        }

        public UniTask Enter(CancellationToken lifetime)
        {
            _screenNavigator.NavigateTo<HudItem, HudScreenPayload>(new());
            _testGameplay.Run();
            _gameplayLoopService.Run();
            return UniTask.CompletedTask;
        }
    }
}