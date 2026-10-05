using System;
using System.Collections.Generic;

namespace DebugVisualization
{
    internal sealed class DebugVizHudController
    {
        private readonly DebugVizRunner _runner;
        private string _currentChannelPath = string.Empty;
        private bool _useChannelValueColors;

        public DebugVizHudController(DebugVizRunner runner)
        {
            _runner = runner;
        }

        public string CurrentChannelPath => _currentChannelPath;
        public bool UseChannelValueColors => _useChannelValueColors;

        public void ResetPath()
        {
            _currentChannelPath = string.Empty;
        }

        public void Refresh()
        {
            View?.RequestRenderFull();
        }

        public DebugVizHudView View { get; set; }

        public void RefreshValuesOnly()
        {
            View?.RequestRenderValuesOnly();
        }

        public void SetUseChannelValueColors(bool enabled)
        {
            if (_useChannelValueColors == enabled)
                return;

            _useChannelValueColors = enabled;
            View?.RequestRenderValuesOnly();
        }

        public void RequestFullRefresh()
        {
            View?.RequestRenderFull();
        }

        public ChannelTreeNode BuildTree()
        {
            return ChannelTreeNode.Build(_runner.ChannelNames);
        }

        public ChannelTreeNode ResolveCurrentNode(ChannelTreeNode root)
        {
            var node = root.Find(_currentChannelPath);
            if (node != null)
                return node;

            _currentChannelPath = string.Empty;
            return root;
        }

        public IReadOnlyList<string> GetBreadcrumbParts()
        {
            if (string.IsNullOrEmpty(_currentChannelPath))
                return Array.Empty<string>();

            return _currentChannelPath.Split('/');
        }

        public void NavigateRoot()
        {
            _currentChannelPath = string.Empty;
            View?.RequestRenderFull();
        }

        public void NavigateTo(string path)
        {
            _currentChannelPath = path ?? string.Empty;
            View?.RequestRenderFull();
        }

        public bool IsChannelEnabled(string channel)
        {
            return _runner.IsChannelEnabled(channel);
        }

        public void SetChannelEnabled(string channel, bool enabled)
        {
            _runner.SetChannelEnabled(channel, enabled);
            View?.RequestRenderFull();
        }

        public bool AreAllChannelsEnabled(ChannelTreeNode node)
        {
            if (node.Channel != null && !_runner.IsChannelEnabled(node.Channel))
                return false;

            for (var i = 0; i < node.Children.Count; i++)
            {
                if (!AreAllChannelsEnabled(node.Children[i]))
                    return false;
            }

            return true;
        }

        public void SetChannelsEnabled(ChannelTreeNode node, bool enabled)
        {
            if (node.Channel != null)
                _runner.SetChannelEnabled(node.Channel, enabled);

            for (var i = 0; i < node.Children.Count; i++)
                SetChannelsEnabled(node.Children[i], enabled);
        }

        public IReadOnlyDictionary<string, List<DebugVizHudValue>> BuildValueGroups()
        {
            var groups = new SortedDictionary<string, List<DebugVizHudValue>>();
            var values = _runner.PersistentValues;

            for (var i = 0; i < values.Count; i++)
            {
                var value = values[i];
                if (!_runner.IsChannelEnabled(value.Channel) || !IsInCurrentPath(value.Channel))
                    continue;

                if (!groups.TryGetValue(value.OwnerName, out var ownerValues))
                {
                    ownerValues = new List<DebugVizHudValue>();
                    groups.Add(value.OwnerName, ownerValues);
                }

                ownerValues.Add(value);
            }

            return groups;
        }

        public DebugVizStyleData GetStyle(string channel)
        {
            return _runner.GetChannelStyle(channel);
        }

        public void ClearDraws()
        {
            _runner.ClearDraws();
        }

        private bool IsInCurrentPath(string channel)
        {
            return string.IsNullOrEmpty(_currentChannelPath)
                   || string.Equals(channel, _currentChannelPath, StringComparison.OrdinalIgnoreCase)
                   || channel.StartsWith(_currentChannelPath + "/", StringComparison.OrdinalIgnoreCase);
        }

        internal sealed class ChannelTreeNode
        {
            public readonly string Name;
            public readonly string FullPath;
            public readonly List<ChannelTreeNode> Children = new();
            public string Channel;

            public bool HasChildren => Children.Count > 0;

            private readonly Dictionary<string, ChannelTreeNode> _childrenByName = new(StringComparer.Ordinal);

            private ChannelTreeNode(string name, string fullPath)
            {
                Name = name;
                FullPath = fullPath;
            }

            public static ChannelTreeNode Build(IReadOnlyList<string> channels)
            {
                var root = new ChannelTreeNode(string.Empty, string.Empty);

                for (var i = 0; i < channels.Count; i++)
                    root.Add(channels[i]);

                Sort(root);
                return root;
            }

            private static void Sort(ChannelTreeNode node)
            {
                node.Children.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
                for (var i = 0; i < node.Children.Count; i++)
                    Sort(node.Children[i]);
            }

            private void Add(string channel)
            {
                var parts = channel.Split('/');
                var current = this;
                var currentPath = string.Empty;

                for (var i = 0; i < parts.Length; i++)
                {
                    var part = string.IsNullOrWhiteSpace(parts[i]) ? DebugViz.DefaultChannel : parts[i];
                    currentPath = string.IsNullOrEmpty(currentPath) ? part : $"{currentPath}/{part}";

                    if (!current._childrenByName.TryGetValue(part, out var child))
                    {
                        child = new ChannelTreeNode(part, currentPath);
                        current._childrenByName.Add(part, child);
                        current.Children.Add(child);
                    }

                    current = child;
                }

                current.Channel = channel;
            }

            public ChannelTreeNode Find(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return this;

                var parts = path.Split('/');
                var current = this;

                for (var i = 0; i < parts.Length; i++)
                {
                    if (!current._childrenByName.TryGetValue(parts[i], out current))
                        return null;
                }

                return current;
            }
        }
    }

    internal readonly struct DebugVizHudValue
    {
        public readonly string OwnerName;
        public readonly string Channel;
        public readonly string Label;
        public readonly object Value;

        public DebugVizHudValue(string ownerName, string channel, string label, object value)
        {
            OwnerName = ownerName;
            Channel = channel;
            Label = string.IsNullOrWhiteSpace(label) ? "Value" : label;
            Value = value;
        }
    }
}
