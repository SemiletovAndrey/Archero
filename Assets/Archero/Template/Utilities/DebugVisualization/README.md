# Debug Visualization

Runtime debug drawing and HUD inspection for editor and development builds.

## Build gating

Public draw methods are decorated with:

- `UNITY_EDITOR`
- `DEVELOPMENT_BUILD`
- `DEBUG_VISUALS`

Calls are compiled in when any of those symbols are present. Production builds without these symbols will strip calls to the `void` debug API methods.

## Choosing the right approach

Use the integration style based on update pattern and ownership:

- `DebugViz` static methods:
  Best for event-driven or one-off debug marks (hit point, path probe, decision marker, etc.).
- Attributes (`[DebugValue]`, `[DebugRadius]`):
  Best for exposing stable object state every frame with minimal code.
- `IDebugDrawable`:
  Best for custom per-object debug drawing logic that is too dynamic/complex for attributes.

In practice:

- Start with attributes for common telemetry fields.
- Add static method calls for important events.
- Implement `IDebugDrawable` when you need composed or conditional draw logic per object.

## Event-based drawing

```csharp
using DebugVisualization;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var point = collision.GetContact(0).point;
        var damage = 25f;

        DebugViz.DrawNumber(point, damage, "Combat/Damage");
        DebugViz.DrawRadius(transform.position, 2.5f, "Combat/Attack Range");
    }
}
```

### Static methods reference (when to use)

- `DebugViz.DrawLabel(...)`: arbitrary world text marker.
- `DebugViz.DrawNumber(...)`: numeric marker (internally a formatted label).
- `DebugViz.DrawLine(...)`: relation/direction between two points.
- `DebugViz.DrawRay(...)`: forward/direction probe from origin.
- `DebugViz.DrawRadius(...)`: ring for range/area boundary.
- `DebugViz.DrawZoneDisc(...)`: filled area marker.
- `DebugViz.SetChannelEnabled(...)`: enable/disable one channel at runtime.
- `DebugViz.ClearDraws()`: clear transient drawn primitives in scene.
- `DebugViz.ClearAll()`: clear draws + registered/persistent debug data.

`duration` behavior:

- `duration: null` means infinite lifetime (persists until cleared).
- `duration: 0f` means draw for current frame/update cycle.
- `duration: > 0f` means timed lifetime in unscaled seconds.

## Attribute-based inspection

`MonoBehaviour` classes with debug attributes are discovered automatically at runtime from loaded assemblies. Manual `DebugViz.Register(this)` / `DebugViz.Unregister(this)` is still available for unusual cases.

```csharp
using DebugVisualization;
using UnityEngine;

public sealed class EnemyDebugView : MonoBehaviour
{
    [SerializeField, DebugValue("AI/Stats", Label = "Move Speed", Display = DebugVizValueDisplay.HudAndWorld)]
    private float _moveSpeed = 4f;

    [SerializeField, DebugRadius("AI/Vision", Label = "Vision Radius", Filled = true, ValueDisplay = DebugVizValueDisplay.World)]
    private float _visionRadius = 8f;
}
```

Display options:

- `DebugVizValueDisplay.Hud`
- `DebugVizValueDisplay.World`
- `DebugVizValueDisplay.HudAndWorld`

World labels are only drawn for `MonoBehaviour` / `GameObject` targets because they need a transform position. Multiple labels on the same object are stacked above the transform.

### Attribute guide

Use `[DebugValue]` for values you want to inspect continuously:

- health, speed, current state, cooldown timers, target id/name
- quick HUD/world visibility without writing draw code

Use `[DebugRadius]` for numeric range values tied to an object position:

- aggro range, interaction radius, sensor radius, weapon range
- optional filled disc for zone-style visualization

Attributes are ideal when the debug output is directly derived from a single field/property.

## `IDebugDrawable` usage

Implement `IDebugDrawable` when debug output depends on logic, multiple values, or branching:

- draw only in certain states
- combine several primitives in one view
- compute derived geometry before drawing

```csharp
using DebugVisualization;
using UnityEngine;

public sealed class AgentDebugView : MonoBehaviour, IDebugDrawable
{
    [SerializeField] private float _visionRadius = 8f;
    [SerializeField] private Transform _target;

    public void DrawDebug(in DebugVizContext context)
    {
        context.Radius(transform.position, _visionRadius, "AI/Vision");

        if (_target != null)
        {
            context.Line(transform.position, _target.position, "AI/Target Link");
            context.Label(_target.position, "Target", "AI/Target Link");
        }

        context.Value("AI/State", "Target Assigned", _target != null);
    }
}
```

Notes:

- `DebugVizContext` helpers (`Label/Line/Radius/ZoneDisc/Value`) keep object ownership wired for HUD/world labels.
- You can still call `DebugViz.*` directly inside `DrawDebug` if needed.
- Prefer `IDebugDrawable` over many attributes once debug logic becomes conditional or cross-field.

## Settings

Create an optional settings asset at:

```text
Assets/Resources/DebugVizSettings.asset
```

Use `Create > Debug Visualization > Settings`. If no asset exists, the tool uses runtime defaults.

## Current runtime renderer

- HUD: IMGUI window with channel toggles and registered values.
- Labels: world-to-screen IMGUI labels.
- Lines/radius/zones: runtime `GL` rendering.

This is intentionally lightweight for the first pass. It can later be swapped for pooled `LineRenderer`, procedural meshes, or TextMeshPro labels without changing gameplay call sites.

## Examples

Use the editor menu:

```text
Tools > Debug Visualization > Create Example Objects
```

It creates a small scene setup with:

- animated targets using `[DebugValue]` and `[DebugRadius]`
- values shown in the HUD and above transforms
- runtime labels, radius rings, filled zones, and forward lines
- a moving collision emitter that draws damage numbers and impact radius
