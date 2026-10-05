using System;

namespace DebugVisualization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DebugValueAttribute : Attribute
    {
        public DebugValueAttribute(string channel = DebugViz.DefaultChannel)
        {
            Channel = channel;
        }

        public string Channel { get; }
        public string Label { get; set; }
        public DebugVizValueDisplay Display { get; set; } = DebugVizValueDisplay.Hud;
        public float WorldYOffset { get; set; } = 2f;
    }
}
