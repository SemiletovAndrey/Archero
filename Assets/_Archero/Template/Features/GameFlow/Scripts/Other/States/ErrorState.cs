using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.GameStateMachine
{
    public class ErrorState: IPayloadState<Exception>
    {
        public UniTask Enter(Exception exception, CancellationToken lifetime)
        {
            Debug.LogError($"[GameFlow] Entered ErrorState: {exception}");
 
            // TODO: показать окно ошибки. Если UI зависит от DI, достань его лениво и в try/catch.
            // Повторный запуск Enter<BootState>() безопасен только если глобальные фичи
            // переживают повторный InitFeature(); иначе перезапускай приложение целиком.
 
            return UniTask.CompletedTask;
        }
 
        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }

}