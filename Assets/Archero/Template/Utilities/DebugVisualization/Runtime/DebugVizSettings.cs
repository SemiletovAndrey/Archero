using System;
using System.Collections.Generic;
using UnityEngine;

namespace DebugVisualization
{
    [CreateAssetMenu(fileName = "DebugVizSettings", menuName = "Debug Visualization/Settings")]
    public sealed class DebugVizSettings : ScriptableObject
    {
        [SerializeField] private bool _showHudOnStart = true;
        [SerializeField, Min(32)] private int _maxTransientDraws = 1024;
        [SerializeField] private DebugVizStyleData _defaultStyle = DebugVizStyleData.Default;
        [SerializeField] private List<DebugVizChannelStyle> _channelStyles = new();

        public bool ShowHudOnStart => _showHudOnStart;
        public int MaxTransientDraws => _maxTransientDraws;
        public DebugVizStyleData DefaultStyle => _defaultStyle;
        public IReadOnlyList<DebugVizChannelStyle> ChannelStyles => _channelStyles;

        public DebugVizStyleData GetStyle(string channel)
        {
            channel = string.IsNullOrWhiteSpace(channel) ? DebugViz.DefaultChannel : channel;
            DebugVizChannelStyle bestMatch = null;

            for (var i = 0; i < _channelStyles.Count; i++)
            {
                if (_channelStyles[i].Matches(channel))
                {
                    if (bestMatch == null || _channelStyles[i].Channel.Length > bestMatch.Channel.Length)
                        bestMatch = _channelStyles[i];
                }
            }

            if (bestMatch != null)
                return bestMatch.Style;

            var createdStyle = new DebugVizChannelStyle(channel, DebugVizStyleData.Default);
            _channelStyles.Add(createdStyle);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif

            return createdStyle.Style;
        }
    }

    [Serializable]
    public sealed class DebugVizChannelStyle
    {
        [SerializeField] private string _channel = "General";
        [SerializeField] private DebugVizStyleData _style = DebugVizStyleData.Default;

        public DebugVizChannelStyle()
        {
        }

        public DebugVizChannelStyle(string channel, DebugVizStyleData style)
        {
            _channel = string.IsNullOrWhiteSpace(channel) ? DebugViz.DefaultChannel : channel;
            _style = style ?? DebugVizStyleData.Default;
        }

        public string Channel => _channel;
        public DebugVizStyleData Style => _style;

        public bool Matches(string channel)
        {
            if (string.IsNullOrWhiteSpace(_channel) || string.IsNullOrWhiteSpace(channel))
                return false;

            if (string.Equals(_channel, channel, StringComparison.OrdinalIgnoreCase))
                return true;

            return channel.Length > _channel.Length
                   && channel.StartsWith(_channel, StringComparison.OrdinalIgnoreCase)
                   && channel[_channel.Length] == '/';
        }
    }
}
