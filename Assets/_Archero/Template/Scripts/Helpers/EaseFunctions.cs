using System;
using Template.Scripts.Enums;
using UnityEngine;

namespace Template.Scripts.Helpers
{
    public class EaseFunctions
    {
        public static Vector3 CustomVectorLerp(Vector3 a, Vector3 b, float t, EaseType easeType)
        {
            Vector3 current = new()
            {
                x = CustomFloatLerp(a.x, b.x, t, easeType),
                y = CustomFloatLerp(a.y, b.y, t, easeType),
                z = CustomFloatLerp(a.z, b.z, t, easeType)
            };

            return current;
        }

        public static float CustomFloatLerp(float a, float b, float t, EaseType easeType)
        {
            return a + (b - a) * CalculateValueByPercentage(t, easeType);
        }

        public static float CustomFloatLerp(float a, float b, float elapsedTime, float duration, EaseType easeType)
        {
            return a + (b - a) * CalculateValueByTime(elapsedTime, duration, easeType);
        }

        public static float CalculateValueByTime(float elapsedTime, float duration, EaseType easeType,
            bool reverse = false)
        {
            float percentage = elapsedTime / duration;
            float value = CalculateValueByPercentage(percentage, easeType, reverse);

            return value;
        }

        public static float CalculateValueByPercentage(float percentage, EaseType easeType, bool reverse = false)
        {
            float value = GetEaseValueBytPercentage(percentage, easeType);

            if (reverse)
            {
                return 1 - value;
            }
            else
            {
                return value;
            }
        }

        private static float GetEaseValueBytPercentage(float value, EaseType easeType)
        {
            return easeType switch
            {
                EaseType.Linear => value,
                EaseType.EaseInOutQuad => EaseInOutQuad(value),
                EaseType.EaseOutExpo => EaseOutExpo(value),
                EaseType.EaseOutCubic => EaseOutCubic(value),
                EaseType.EaseInOutQuint => EaseInOutQuint(value),
                EaseType.EaseOutBack => EaseOutBack(value),
                _ => 0,
            };
        }

        private static float EaseInOutQuad(float value)
        {
            return (float)(value < 0.5 ? 2 * value * value : 1 - Math.Pow(-2 * value + 2, 2) / 2);
        }

        private static float EaseOutExpo(float value)
        {
            return value == 1 ? 1 : 1 - (float)Math.Pow(2, -10 * value);
        }

        private static float EaseOutCubic(float value)
        {
            return 1 - (float)Math.Pow(1 - value, 3);
        }

        private static float EaseInOutQuint(float value)
        {
            return value < 0.5
                ? 16 * value * value * value * value * value
                : 1 - (float)Math.Pow(-2 * value + 2, 5) / 2;
        }

        private static float EaseOutBack(float value)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            return 1 + c3 * Mathf.Pow(value - 1, 3) + c1 * Mathf.Pow(value - 1, 2);
        }
    }
}