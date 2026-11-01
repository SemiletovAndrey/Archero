using System;
using UnityEngine;

namespace DebugVisualization
{
    [Serializable]
    public class DebugVizStyleData
    {
        public Color Color = Color.white;
        public Color FillColor = new(1f, 1f, 1f, 0.12f);
        [Min(1f)] public float TextSize = 18f;
        [Min(0.001f)] public float LineWidth = 0.03f;
        [Min(0f)] public float Duration = 1f;
        public bool DepthTest = true;

        public static DebugVizStyleData Default => new();

        public Color VisibleColor
        {
            get
            {
                if (Color.a > 0f)
                    return Color;

                return new Color(Color.r, Color.g, Color.b, 1f);
            }
        }

        public Color VisibleFillColor
        {
            get
            {
                if (FillColor.a > 0f)
                    return FillColor;

                return new Color(FillColor.r, FillColor.g, FillColor.b, 0.12f);
            }
        }

        public DebugVizStyleData Clone()
        {
            return new DebugVizStyleData
            {
                Color = Color,
                FillColor = FillColor,
                TextSize = TextSize,
                LineWidth = LineWidth,
                Duration = Duration,
                DepthTest = DepthTest
            };
        }
    }

    [CreateAssetMenu(fileName = "DebugVizStyle", menuName = "Debug Visualization/Style")]
    public sealed class DebugVizStyle : ScriptableObject
    {
        [SerializeField] private DebugVizStyleData _data = DebugVizStyleData.Default;

        public DebugVizStyleData Data => _data;
    }
}
