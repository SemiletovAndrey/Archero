using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.GameStateMachine;
using UnityEngine;

namespace Gameplay.Scripts
{
    public class GameplaySceneEntryPoint : ISceneEntryPoint
    {
        public UniTask Enter(CancellationToken lifetime)
        {
            Debug.Log("Enter on scene gameplay");
            return UniTask.CompletedTask;
        }
    }
}