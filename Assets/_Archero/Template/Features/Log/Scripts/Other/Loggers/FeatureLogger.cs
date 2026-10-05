using System;
using UnityEngine;

namespace Template.Features.Log
{
    public class FeatureLogger : IDebugLogger
    {
        private readonly string _featureName;
        private readonly DebugLogStyleData _resolvedStyle;

        public FeatureLogger(string featureName, DebugLogStyleData resolvedStyle)
        {
            _featureName = string.IsNullOrWhiteSpace(featureName) ? "General" : featureName;
            _resolvedStyle = resolvedStyle ?? DebugLogStyleData.Default;
        }

        public void LogMessage(string message, DebugLogStyleData styleOverride = null)
        {
            DebugLogger.LogMessage(ExpandMessage(message, styleOverride));
        }

        public void LogWarning(string message, DebugLogStyleData styleOverride = null)
        {
            DebugLogger.LogWarning(ExpandMessage(message, styleOverride));
        }

        public void LogError(string message, DebugLogStyleData styleOverride = null)
        {
            DebugLogger.LogError(ExpandMessage(message, styleOverride));
        }

        public void LogException(Exception exception)
        {
            DebugLogger.LogException(exception);
        }

        private string ExpandMessage(string message, DebugLogStyleData styleOverride)
        {
            var style = styleOverride ?? _resolvedStyle;
            var safeMessage = message ?? string.Empty;
            var featureText = ApplyColor($"[{_featureName}]", style.FeatureColor);
            var debugText = ApplyColor(safeMessage, style.TextColor);
            var combined = $"{featureText} {debugText}";
            return ApplyTextStyle(combined, style);
        }

        private static string ApplyColor(string text, Color color)
        {
            var hex = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{hex}>{text}</color>";
        }

        private static string ApplyTextStyle(string text, DebugLogStyleData style)
        {
            var styled = text;
            styled = $"<size={Mathf.Max(1, style.TextSize)}>{styled}</size>";

            if (style.Bold)
                styled = $"<b>{styled}</b>";
            if (style.Italic)
                styled = $"<i>{styled}</i>";
            if (style.Underline)
                styled = $"<u>{styled}</u>";

            return styled;
        }
    }
}
