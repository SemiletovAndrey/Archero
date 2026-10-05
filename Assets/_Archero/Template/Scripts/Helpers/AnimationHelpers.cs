using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Scripts.Enums;
using Template.Scripts.Helpers;
using UnityEngine;

namespace Template.Scripts.Utils
{
    public static class AnimationHelpers
    {
        public static async UniTask PlayAnimation(Action<float> setter, float startValue, float endValue,
            float duration, CancellationToken cancellationToken, EaseType easeType = EaseType.Linear)
        {
            float elapsedTime = 0;
            while (elapsedTime < duration && !cancellationToken.IsCancellationRequested)
            {
                float current = EaseFunctions.CustomFloatLerp(startValue, endValue, elapsedTime, duration, easeType);
                setter(current);

                elapsedTime += Time.deltaTime;

                await UniTask.NextFrame();
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                setter(endValue);
            }
        }

        public static UniTask PlayAnimation(Action<float> setter, float duration,
            CancellationToken cancellationToken, EaseType easeType = EaseType.Linear)
        {
            return PlayAnimation(setter, 0f, 1f, duration, cancellationToken, easeType);
        }

        public static async UniTask PlayAnimationWithCurrentValue(float currentValue, Action<float> setter, 
            float startValue, float endValue, float duration, CancellationToken cancellationToken,
            EaseType easeType = EaseType.Linear)
        {
            float left = endValue - currentValue;
            float delta = endValue - startValue;
            float percentage = 1 - (left / delta);

            float elapsedTime = percentage * duration;
            while (elapsedTime < duration && !cancellationToken.IsCancellationRequested)
            {
                float current = EaseFunctions.CustomFloatLerp(startValue, endValue, elapsedTime, duration, easeType);
                setter(current);

                elapsedTime += Time.deltaTime;

                await UniTask.NextFrame();
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                setter(endValue);
            }
        }
        
        public static UniTask PlayAnimationWithCurrentValue(float currentValue, Action<float> setter, 
            float duration, CancellationToken cancellationToken, EaseType easeType = EaseType.Linear)
        {
            return PlayAnimationWithCurrentValue(currentValue, setter, 0f, 1f, duration, cancellationToken, easeType);
        }

        public static float CalculateRemainingDuration(float initialDuration, float currentValue, float startValue,
            float endValue)
        {
            float left = endValue - currentValue;
            float delta = endValue - startValue;

            float percentage = left / delta;

            return percentage * initialDuration;
        }

        public static async UniTask PlayAnimationVector2(Vector2 startValue, Action<Vector2> setter, Vector2 endValue,
            float duration, CancellationToken cancellationToken, EaseType easeType = EaseType.Linear)
        {
            float elapsed = 0;
            while (elapsed < duration && !cancellationToken.IsCancellationRequested)
            {
                float t = elapsed / duration;
                float x = EaseFunctions.CustomFloatLerp(startValue.x, endValue.x, t, easeType);
                float y = EaseFunctions.CustomFloatLerp(startValue.y, endValue.y, t, easeType);
                setter(new Vector2(x, y));

                elapsed += Time.deltaTime;
                await UniTask.NextFrame();
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                setter(endValue);
            }
        }
    }
}