using System;

namespace DebugVisualization
{
    [Flags]
    public enum DebugVizValueDisplay
    {
        None = 0,
        Hud = 1,
        World = 2,
        HudAndWorld = Hud | World
    }
}
