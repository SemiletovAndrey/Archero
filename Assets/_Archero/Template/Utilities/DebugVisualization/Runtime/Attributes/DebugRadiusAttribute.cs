using System;

namespace DebugVisualization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DebugRadiusAttribute : Attribute
    {
        public DebugRadiusAttribute(string channel = DebugViz.DefaultChannel)
        {
            Channel = channel;
        }

        public string Channel { get; }
        public string Label { get; set; }
        public bool Filled { get; set; }
        public DebugVizValueDisplay ValueDisplay { get; set; } = DebugVizValueDisplay.Hud;
        public float WorldYOffset { get; set; } = 2f;
    }
}
