using System;
using System.Collections.Generic;
using UnityEngine;

namespace Template.Features.Log
{
    [CreateAssetMenu(fileName = "DebugLogSettings", menuName = "Log/Debug Log Settings")]
    public sealed class DebugLogSettings : ScriptableObject
    {
        [SerializeField] private DebugLogStyleData _defaultStyle = DebugLogStyleData.Default;
        [SerializeField] private List<DebugLogFeatureStyle> _featureStyles = new();

        public DebugLogStyleData DefaultStyle => _defaultStyle;
        public IReadOnlyList<DebugLogFeatureStyle> FeatureStyles => _featureStyles;

        public DebugLogStyleData GetStyle(string featureName)
        {
            featureName = string.IsNullOrWhiteSpace(featureName) ? "General" : featureName;
            DebugLogFeatureStyle bestMatch = null;

            for (var i = 0; i < _featureStyles.Count; i++)
            {
                if (_featureStyles[i].Matches(featureName))
                {
                    if (bestMatch == null || _featureStyles[i].Feature.Length > bestMatch.Feature.Length)
                        bestMatch = _featureStyles[i];
                }
            }

            if (bestMatch != null)
                return bestMatch.Style;

            var createdStyle = new DebugLogFeatureStyle(featureName, DebugLogStyleData.Default);
            _featureStyles.Add(createdStyle);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            return createdStyle.Style;
        }
    }

    [Serializable]
    public sealed class DebugLogFeatureStyle
    {
        [SerializeField] private string _feature = "General";
        [SerializeField] private DebugLogStyleData _style = DebugLogStyleData.Default;

        public DebugLogFeatureStyle()
        {
        }

        public DebugLogFeatureStyle(string feature, DebugLogStyleData style)
        {
            _feature = string.IsNullOrWhiteSpace(feature) ? "General" : feature;
            _style = style ?? DebugLogStyleData.Default;
        }

        public string Feature => _feature;
        public DebugLogStyleData Style => _style;

        public bool Matches(string feature)
        {
            if (string.IsNullOrWhiteSpace(_feature) || string.IsNullOrWhiteSpace(feature))
                return false;

            if (string.Equals(_feature, feature, StringComparison.OrdinalIgnoreCase))
                return true;

            return feature.Length > _feature.Length
                   && feature.StartsWith(_feature, StringComparison.OrdinalIgnoreCase)
                   && feature[_feature.Length] == '/';
        }
    }
}
