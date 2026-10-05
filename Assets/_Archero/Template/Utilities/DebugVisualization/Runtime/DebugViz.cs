using System.Diagnostics;
using UnityEngine;

namespace DebugVisualization
{
    public static class DebugViz
    {
        public const string DefaultChannel = "General";

        public static bool IsAvailable
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD || DEBUG_VISUALS
                return true;
#else
                return false;
#endif
            }
        }

        public static bool HudVisible
        {
            get => DebugVizRuntime.HudVisible;
            set => DebugVizRuntime.HudVisible = value;
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void DrawNumber(Vector3 position, float value, string channel = DefaultChannel, DebugVizStyle style = null, float? duration = null)
        {
            DrawLabel(position, value.ToString("0.##"), channel, style, duration);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void DrawLabel(Vector3 position, string text, string channel = DefaultChannel, DebugVizStyle style = null, float? duration = null)
        {
            DebugVizRuntime.Add(DebugVizDrawCommand.Label(position, text, NormalizeChannel(channel), ResolveStyle(channel, style), duration));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void DrawLine(Vector3 start, Vector3 end, string channel = DefaultChannel, DebugVizStyle style = null, float? duration = null)
        {
            DebugVizRuntime.Add(DebugVizDrawCommand.Line(start, end, NormalizeChannel(channel), ResolveStyle(channel, style), duration));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void DrawRay(Vector3 start, Vector3 direction, string channel = DefaultChannel, DebugVizStyle style = null, float? duration = null)
        {
            DrawLine(start, start + direction, channel, style, duration);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void DrawRadius(Vector3 center, float radius, string channel = DefaultChannel, DebugVizStyle style = null, float? duration = null, int segments = 64)
        {
            DebugVizRuntime.Add(DebugVizDrawCommand.Circle(center, radius, NormalizeChannel(channel), ResolveStyle(channel, style), duration, Mathf.Max(8, segments)));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void DrawZoneDisc(Vector3 center, float radius, string channel = DefaultChannel, DebugVizStyle style = null, float? duration = null, int segments = 64)
        {
            DebugVizRuntime.Add(DebugVizDrawCommand.Disc(center, radius, NormalizeChannel(channel), ResolveStyle(channel, style), duration, Mathf.Max(8, segments)));
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void SetChannelEnabled(string channel, bool enabled)
        {
            DebugVizRuntime.SetChannelEnabled(NormalizeChannel(channel), enabled);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void Register(Object target)
        {
            DebugVizRegistry.Register(target);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void Unregister(Object target)
        {
            DebugVizRegistry.Unregister(target);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void ClearAll()
        {
            DebugVizRuntime.ClearAll();
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void ClearDraws()
        {
            DebugVizRuntime.ClearDraws();
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        [Conditional("DEBUG_VISUALS")]
        public static void Shutdown()
        {
            DebugVizRuntime.Shutdown();
        }

        private static string NormalizeChannel(string channel)
        {
            return string.IsNullOrWhiteSpace(channel) ? DefaultChannel : channel;
        }

        private static DebugVizStyleData ResolveStyle(string channel, DebugVizStyle style)
        {
            return style != null ? style.Data : DebugVizRuntime.Settings.GetStyle(NormalizeChannel(channel));
        }
    }
}
