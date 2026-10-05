using System.Collections.Generic;
using UnityEngine;

namespace DebugVisualization
{
    internal sealed class DebugVizRunner : MonoBehaviour
    {
        private readonly List<DebugVizDrawCommand> _draws = new();
        private readonly List<DebugVizHudValue> _persistentValues = new();
        private readonly List<PersistentWorldLabel> _persistentWorldLabels = new();
        private readonly Dictionary<Object, int> _worldLabelStack = new();
        private readonly Dictionary<Object, WorldOwnerLabel> _worldOwnerLabels = new();
        private readonly Dictionary<string, bool> _channels = new();
        private readonly List<string> _channelNames = new();

        private DebugVizSettings _settings;
        private Material _lineMaterial;
        private float _nextSceneScanTime;
        private float _nextHudValuesRefreshTime;
        private bool _hudVisible;

        private DebugVizHudController _hudController;
        private DebugVizHudView _hudView;

        public bool HudVisible
        {
            get => _hudVisible;
            set
            {
                _hudVisible = value;
                UpdateHudVisibility();
            }
        }

        public IReadOnlyCollection<string> Channels => _channelNames;
        internal IReadOnlyList<string> ChannelNames => _channelNames;
        internal IReadOnlyList<DebugVizHudValue> PersistentValues => _persistentValues;

        public void Initialize(DebugVizSettings settings)
        {
            _settings = settings;
            _hudController = new DebugVizHudController(this);
            _hudView = new DebugVizHudView(_hudController);
            _hudView.Attach(gameObject);
            _hudController.Refresh();
            HudVisible = settings.ShowHudOnStart;
        }

        public void Add(DebugVizDrawCommand command)
        {
            RegisterChannel(command.Channel);

            if (_draws.Count >= _settings.MaxTransientDraws)
                _draws.RemoveAt(0);

            _draws.Add(command);
        }

        public void AddPersistentValue(Object owner, string channel, string label, object value)
        {
            channel = string.IsNullOrWhiteSpace(channel) ? DebugViz.DefaultChannel : channel;
            RegisterChannel(channel);
            _persistentValues.Add(new DebugVizHudValue(owner != null ? owner.name : "Unknown", channel, label, value));
        }

        public void AddPersistentWorldLabel(Object owner, string channel, string label, object value, Vector3 position)
        {
            channel = string.IsNullOrWhiteSpace(channel) ? DebugViz.DefaultChannel : channel;
            RegisterChannel(channel);
            _persistentWorldLabels.Add(new PersistentWorldLabel(owner, channel, label, value, position));
        }

        public bool IsChannelEnabled(string channel)
        {
            channel = string.IsNullOrWhiteSpace(channel) ? DebugViz.DefaultChannel : channel;
            return !_channels.TryGetValue(channel, out var enabled) || enabled;
        }

        public void SetChannelEnabled(string channel, bool enabled)
        {
            channel = string.IsNullOrWhiteSpace(channel) ? DebugViz.DefaultChannel : channel;
            RegisterChannel(channel);
            _channels[channel] = enabled;
        }

        public void ClearAll()
        {
            _draws.Clear();
            _persistentValues.Clear();
            _persistentWorldLabels.Clear();
            _worldLabelStack.Clear();
            _worldOwnerLabels.Clear();
            _channels.Clear();
            _channelNames.Clear();
            _hudController?.ResetPath();
            if (Time.unscaledTime >= _nextHudValuesRefreshTime)
            {
                _nextHudValuesRefreshTime = Time.unscaledTime + 0.1f;
                _hudController?.RefreshValuesOnly();
            }
        }

        public void ClearDraws()
        {
            _draws.Clear();
            _hudController?.Refresh();
        }

        internal DebugVizStyleData GetChannelStyle(string channel)
        {
            return _settings.GetStyle(channel);
        }

        private void OnDestroy()
        {
            _hudView?.Dispose();
            ClearAll();
        }

        private void LateUpdate()
        {
            if (Time.unscaledTime >= _nextSceneScanTime)
            {
                _nextSceneScanTime = Time.unscaledTime + 0.5f;
                DebugVizRegistry.AutoRegisterSceneTargets();
            }

            _persistentValues.Clear();
            _persistentWorldLabels.Clear();
            DebugVizRegistry.Refresh();

            for (var i = _draws.Count - 1; i >= 0; i--)
            {
                if (_draws[i].IsExpired)
                    _draws.RemoveAt(i);
            }

            _hudController?.Refresh();
        }

        private void OnRenderObject()
        {
            if (_draws.Count == 0)
                return;

            EnsureLineMaterial();
            _lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);

            for (var i = 0; i < _draws.Count; i++)
            {
                var draw = _draws[i];

                if (!IsChannelEnabled(draw.Channel))
                    continue;

                ApplyMaterialDepth(draw.Style);

                switch (draw.Kind)
                {
                    case DebugVizDrawKind.Line:
                        DrawLine(draw);
                        break;
                    case DebugVizDrawKind.Circle:
                        DrawCircle(draw);
                        break;
                    case DebugVizDrawKind.Disc:
                        DrawDisc(draw);
                        break;
                }
            }

            GL.PopMatrix();
        }

        private void OnGUI()
        {
            DrawWorldLabels();
        }

        private void UpdateHudVisibility()
        {
            _hudView?.SetVisible(_hudVisible);
        }

        private void DrawWorldLabels()
        {
            var camera = Camera.main;
            if (camera == null)
                return;

            DrawPersistentWorldLabels(camera);
            DrawTransientWorldLabels(camera);
        }

        private void DrawPersistentWorldLabels(Camera camera)
        {
            _worldLabelStack.Clear();
            _worldOwnerLabels.Clear();

            for (var i = 0; i < _persistentWorldLabels.Count; i++)
            {
                var label = _persistentWorldLabels[i];

                if (!IsChannelEnabled(label.Channel))
                    continue;

                var stackIndex = 0;
                if (label.Owner != null)
                {
                    _worldLabelStack.TryGetValue(label.Owner, out stackIndex);
                    _worldLabelStack[label.Owner] = stackIndex + 1;
                }

                if (DrawScreenLabel(camera, label.Position, $"{label.Label}: {FormatValue(label.Value)}", _settings.GetStyle(label.Channel), stackIndex, out var rect)
                    && label.Owner != null)
                {
                    _worldOwnerLabels[label.Owner] = WorldOwnerLabel.Combine(
                        _worldOwnerLabels.TryGetValue(label.Owner, out var existing) ? existing : default,
                        label.Owner.name,
                        label.Channel,
                        rect);
                }
            }

            foreach (var pair in _worldOwnerLabels)
            {
                var owner = pair.Key;
                var label = pair.Value;

                if (owner == null || !label.HasValue || !IsChannelEnabled(label.Channel))
                    continue;

                DrawScreenLabelAboveRect(label.OwnerName, label.TopRect, _settings.GetStyle(label.Channel));
            }
        }

        private void DrawTransientWorldLabels(Camera camera)
        {
            for (var i = 0; i < _draws.Count; i++)
            {
                var draw = _draws[i];

                if (draw.Kind != DebugVizDrawKind.Label || !IsChannelEnabled(draw.Channel))
                    continue;

                DrawScreenLabel(camera, draw.A, draw.Text, draw.Style, 0, out _);
            }
        }

        private static bool DrawScreenLabel(Camera camera, Vector3 worldPosition, string text, DebugVizStyleData style, int stackIndex, out Rect rect)
        {
            var screenPoint = camera.WorldToScreenPoint(worldPosition);
            if (screenPoint.z <= 0f)
            {
                rect = default;
                return false;
            }

            var previousColor = GUI.color;
            var previousSize = GUI.skin.label.fontSize;
            GUI.color = style.VisibleColor;
            GUI.skin.label.fontSize = Mathf.RoundToInt(style.TextSize);

            var size = GUI.skin.label.CalcSize(new GUIContent(text));
            var y = Screen.height - screenPoint.y - size.y * 0.5f - stackIndex * (size.y + 2f);
            rect = new Rect(screenPoint.x - size.x * 0.5f, y, size.x, size.y);
            GUI.Label(rect, text);

            GUI.skin.label.fontSize = previousSize;
            GUI.color = previousColor;
            return true;
        }

        private static void DrawScreenLabelAboveRect(string text, Rect topRect, DebugVizStyleData style)
        {
            var previousColor = GUI.color;
            var previousSize = GUI.skin.label.fontSize;
            GUI.color = style.VisibleColor;
            GUI.skin.label.fontSize = Mathf.RoundToInt(style.TextSize);

            var size = GUI.skin.label.CalcSize(new GUIContent(text));
            var rect = new Rect(topRect.center.x - size.x * 0.5f, topRect.yMin - size.y - 2f, size.x, size.y);
            GUI.Label(rect, text);

            GUI.skin.label.fontSize = previousSize;
            GUI.color = previousColor;
        }

        private void DrawLine(DebugVizDrawCommand draw)
        {
            DrawThickLine(draw.A, draw.B, draw.Style);
        }

        private void DrawThickLine(Vector3 start, Vector3 end, DebugVizStyleData style)
        {
            var direction = end - start;
            if (direction.sqrMagnitude <= 0.0001f)
                return;

            var camera = Camera.current != null ? Camera.current : Camera.main;
            var cameraForward = camera != null ? camera.transform.forward : Vector3.forward;
            var normal = Vector3.Cross(direction.normalized, cameraForward).normalized;

            if (normal.sqrMagnitude <= 0.0001f)
                normal = Vector3.Cross(direction.normalized, Vector3.up).normalized;

            var halfWidth = Mathf.Max(0.001f, style.LineWidth) * 0.5f;
            normal *= halfWidth;

            GL.Begin(GL.QUADS);
            GL.Color(style.VisibleColor);
            GL.Vertex(start - normal);
            GL.Vertex(start + normal);
            GL.Vertex(end + normal);
            GL.Vertex(end - normal);
            GL.End();
        }

        private void DrawCircle(DebugVizDrawCommand draw)
        {
            var previous = PointOnCircle(draw.A, draw.Radius, 0f);

            for (var i = 1; i <= draw.Segments; i++)
            {
                var angle = i / (float)draw.Segments * Mathf.PI * 2f;
                var next = PointOnCircle(draw.A, draw.Radius, angle);
                DrawThickLine(previous, next, draw.Style);
                previous = next;
            }
        }

        private void DrawDisc(DebugVizDrawCommand draw)
        {
            GL.Begin(GL.TRIANGLES);
            GL.Color(draw.Style.VisibleFillColor);

            for (var i = 0; i < draw.Segments; i++)
            {
                var currentAngle = i / (float)draw.Segments * Mathf.PI * 2f;
                var nextAngle = (i + 1) / (float)draw.Segments * Mathf.PI * 2f;

                GL.Vertex(draw.A);
                GL.Vertex(PointOnCircle(draw.A, draw.Radius, currentAngle));
                GL.Vertex(PointOnCircle(draw.A, draw.Radius, nextAngle));
            }

            GL.End();
            DrawCircle(draw);
        }

        private static Vector3 PointOnCircle(Vector3 center, float radius, float angle)
        {
            return center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }

        private void RegisterChannel(string channel)
        {
            if (_channels.ContainsKey(channel))
                return;

            _channels[channel] = true;
            _channelNames.Add(channel);
            _channelNames.Sort();
            _hudController?.Refresh();
        }

        private void EnsureLineMaterial()
        {
            if (_lineMaterial != null)
                return;

            var shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _lineMaterial.SetInt("_ZWrite", 0);
        }

        private void ApplyMaterialDepth(DebugVizStyleData style)
        {
            EnsureLineMaterial();
            _lineMaterial.SetInt("_ZTest", style.DepthTest
                ? (int)UnityEngine.Rendering.CompareFunction.LessEqual
                : (int)UnityEngine.Rendering.CompareFunction.Always);
        }

        private static string FormatValue(object value)
        {
            return value switch
            {
                null => "null",
                float number => number.ToString("0.###"),
                double number => number.ToString("0.###"),
                _ => value.ToString()
            };
        }

        private readonly struct PersistentWorldLabel
        {
            public readonly Object Owner;
            public readonly string Channel;
            public readonly string Label;
            public readonly object Value;
            public readonly Vector3 Position;

            public PersistentWorldLabel(Object owner, string channel, string label, object value, Vector3 position)
            {
                Owner = owner;
                Channel = channel;
                Label = string.IsNullOrWhiteSpace(label) ? (owner != null ? owner.name : "Value") : label;
                Value = value;
                Position = position;
            }
        }

        private readonly struct WorldOwnerLabel
        {
            public readonly string OwnerName;
            public readonly string Channel;
            public readonly Rect TopRect;
            public readonly bool HasValue;

            private WorldOwnerLabel(string ownerName, string channel, Rect topRect)
            {
                OwnerName = ownerName;
                Channel = channel;
                TopRect = topRect;
                HasValue = true;
            }

            public static WorldOwnerLabel Combine(WorldOwnerLabel current, string ownerName, string channel, Rect rect)
            {
                if (!current.HasValue || rect.yMin < current.TopRect.yMin)
                    return new WorldOwnerLabel(ownerName, channel, rect);

                return current;
            }
        }
    }
}
