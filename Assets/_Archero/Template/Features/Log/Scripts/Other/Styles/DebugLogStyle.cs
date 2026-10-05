using System;
using UnityEngine;

namespace Template.Features.Log
{
    [Serializable]
    public class DebugLogStyleData
    {
        public Color FeatureColor = new(0.65f, 0.85f, 1f, 1f);
        public Color TextColor = Color.white;
        [Min(1)] public int TextSize = 12;
        public bool Bold;
        public bool Italic;
        public bool Underline;

        public DebugLogStyleData(Color featureColor = default)
        {
            
        }
        
        public static readonly DebugLogStyleData Default = new();
        

        public DebugLogStyleData Clone()
        {
            return new DebugLogStyleData
            {
                FeatureColor = FeatureColor,
                TextColor = TextColor,
                TextSize = TextSize,
                Bold = Bold,
                Italic = Italic,
                Underline = Underline
            };
        }
    }

    [CreateAssetMenu(fileName = "DebugLogStyle", menuName = "Log/Debug Log Style")]
    public sealed class DebugLogStyle : ScriptableObject
    {
        [SerializeField] private DebugLogStyleData _data = DebugLogStyleData.Default;
        public DebugLogStyleData Data => _data;
    }
}
