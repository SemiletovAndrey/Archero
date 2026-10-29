using System.Threading;
using Cysharp.Threading.Tasks;
using Gameplay.Features.Hud;
using Template.Features.GameStateMachine;
using Template.Features.ScreenNavigator;
using UnityEngine;

namespace Gameplay.Scripts
{
    public class GameplaySceneEntryPoint : ISceneEntryPoint
    {
        private readonly IScreenNavigator _screenNavigator;

        public GameplaySceneEntryPoint(IScreenNavigator screenNavigator)
        {
            _screenNavigator = screenNavigator;
        }

        public UniTask Enter(CancellationToken lifetime)
        {
            _screenNavigator.NavigateTo<HudItem, HudScreenPayload>(new());
            Debug.Log("Enter on scene gameplay");
            return UniTask.CompletedTask;
        }
    }
}