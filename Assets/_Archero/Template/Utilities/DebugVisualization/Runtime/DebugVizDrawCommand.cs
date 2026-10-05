using UnityEngine;

namespace DebugVisualization
{
    internal enum DebugVizDrawKind
    {
        Label,
        Line,
        Circle,
        Disc
    }

    internal readonly struct DebugVizDrawCommand
    {
        public readonly DebugVizDrawKind Kind;
        public readonly string Channel;
        public readonly DebugVizStyleData Style;
        public readonly Vector3 A;
        public readonly Vector3 B;
        public readonly float Radius;
        public readonly int Segments;
        public readonly string Text;
        public readonly float ExpiresAt;

        private DebugVizDrawCommand(DebugVizDrawKind kind, Vector3 a, Vector3 b, float radius, string text, string channel, DebugVizStyleData style, float? duration, int segments)
        {
            Kind = kind;
            A = a;
            B = b;
            Radius = radius;
            Text = text;
            Channel = channel;
            Style = style.Clone();
            Segments = segments;
            ExpiresAt = duration.HasValue
                ? Time.unscaledTime + Mathf.Max(0f, duration.Value)
                : float.PositiveInfinity;
        }

        public bool IsExpired => Time.unscaledTime > ExpiresAt;

        public static DebugVizDrawCommand Label(Vector3 position, string text, string channel, DebugVizStyleData style, float? duration)
        {
            return new DebugVizDrawCommand(DebugVizDrawKind.Label, position, default, 0f, text, channel, style, duration, 0);
        }

        public static DebugVizDrawCommand Line(Vector3 start, Vector3 end, string channel, DebugVizStyleData style, float? duration)
        {
            return new DebugVizDrawCommand(DebugVizDrawKind.Line, start, end, 0f, null, channel, style, duration, 0);
        }

        public static DebugVizDrawCommand Circle(Vector3 center, float radius, string channel, DebugVizStyleData style, float? duration, int segments)
        {
            return new DebugVizDrawCommand(DebugVizDrawKind.Circle, center, default, Mathf.Max(0f, radius), null, channel, style, duration, segments);
        }

        public static DebugVizDrawCommand Disc(Vector3 center, float radius, string channel, DebugVizStyleData style, float? duration, int segments)
        {
            return new DebugVizDrawCommand(DebugVizDrawKind.Disc, center, default, Mathf.Max(0f, radius), null, channel, style, duration, segments);
        }
    }
}
