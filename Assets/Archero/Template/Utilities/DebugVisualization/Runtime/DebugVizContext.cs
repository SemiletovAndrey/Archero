using UnityEngine;

namespace DebugVisualization
{
    public readonly struct DebugVizContext
    {
        private readonly Object _owner;

        public DebugVizContext(Object owner)
        {
            _owner = owner;
        }

        public void Value(string channel, string label, object value)
        {
            DebugVizRuntime.AddPersistentValue(_owner, channel, label, value);
        }

        public void Label(Vector3 position, string text, string channel = DebugViz.DefaultChannel)
        {
            DebugViz.DrawLabel(position, text, channel, duration: 0f);
        }

        public void Radius(Vector3 center, float radius, string channel = DebugViz.DefaultChannel)
        {
            DebugViz.DrawRadius(center, radius, channel, duration: 0f);
        }

        public void ZoneDisc(Vector3 center, float radius, string channel = DebugViz.DefaultChannel)
        {
            DebugViz.DrawZoneDisc(center, radius, channel, duration: 0f);
        }
        
        public void Line(Vector3 start, Vector3 end, string channel = DebugViz.DefaultChannel)
        {
            DebugViz.DrawLine(start, end, channel, duration: 0f);
        }
    }
}
