#if  UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using DataEditorFramework.Core.Data.Attribute;
using UnityEngine;
using UnityEngine.UIElements;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    public class TabController
    {
        private const string OpenTabsPrefsKey = "DataEditorFramework.OpenTabs";
        private const string ActiveTabPrefsKey = "DataEditorFramework.ActiveTab";

        public VisualElement Root { get; } = new();

        private readonly VisualElement _tabStrip = new();
        private readonly VisualElement _content = new();
        private readonly Dictionary<Type, TabState> _tabs = new();
        private readonly List<Type> _tabOrder = new();
        private Type _activeType;

        public TabController()
        {
            Root.AddToClassList("workspace");

            _tabStrip.AddToClassList("tab-strip");
            _content.AddToClassList("tab-content");

            Root.Add(_tabStrip);
            Root.Add(_content);
        }

        public void OpenTab(Type type)
        {
            if (_tabs.ContainsKey(type))
            {
                ActivateTab(type);
                return;
            }

            var view = new DataEditor(type);
            view.Root.style.display = DisplayStyle.None;

            var tab = new VisualElement();
            tab.AddToClassList("tab");

            var tabButton = new Button(() => ActivateTab(type)) { text = type.Name };
            tabButton.AddToClassList("tab-button");
            tabButton.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Close All", _ => CloseAllTabs());
                evt.menu.AppendAction("Close Others", _ => CloseAllTabsExcept(type));
            }));

            var closeButton = new Button(() => CloseTab(type)) { text = "x" };
            closeButton.AddToClassList("tab-close-button");

            tabButton.Add(closeButton);
            tab.Add(tabButton);

            _tabs[type] = new TabState(view, tab);
            _tabOrder.Add(type);

            _tabStrip.Add(tab);
            _content.Add(view.Root);

            ActivateTab(type);
            SaveTabSession();
        }

        private void CloseTab(Type type)
        {
            if (!_tabs.TryGetValue(type, out var tab))
                return;

            var closedIndex = _tabOrder.IndexOf(type);
            var wasActive = _activeType == type;
            tab.View.SavePendingChanges();

            tab.Tab.RemoveFromHierarchy();
            tab.View.Root.RemoveFromHierarchy();
            _tabs.Remove(type);
            _tabOrder.Remove(type);

            if (!wasActive)
            {
                SaveTabSession();
                return;
            }

            _activeType = null;

            if (_tabOrder.Count == 0)
            {
                SaveTabSession();
                return;
            }

            var nextIndex = Math.Min(closedIndex, _tabOrder.Count - 1);
            ActivateTab(_tabOrder[nextIndex]);
            SaveTabSession();
        }

        private void CloseAllTabs()
        {
            foreach (var type in _tabOrder.ToArray())
                CloseTab(type);
        }

        private void CloseAllTabsExcept(Type selectedType)
        {
            if (!_tabs.ContainsKey(selectedType))
                return;

            ActivateTab(selectedType);

            foreach (var type in _tabOrder.ToArray())
            {
                if (type != selectedType)
                    CloseTab(type);
            }
        }

        private void ActivateTab(Type type)
        {
            if (_activeType == type)
                return;

            if (_activeType != null && _tabs.TryGetValue(_activeType, out var previous))
            {
                previous.View.SavePendingChanges();
                previous.View.Root.style.display = DisplayStyle.None;
                previous.Tab.RemoveFromClassList("tab--active");
            }

            var active = _tabs[type];
            active.View.Root.style.display = DisplayStyle.Flex;
            active.Tab.AddToClassList("tab--active");
            _activeType = type;
            SaveTabSession();
        }

        public void SaveAllTabs()
        {
            foreach (var tab in _tabs.Values)
                tab.View.SavePendingChanges();
        }

        public void SaveTabSession()
        {
            var session = new TabSessionData
            {
                openTabs = _tabOrder
                    .Select(t => t.AssemblyQualifiedName)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .ToArray(),
                activeTab = _activeType?.AssemblyQualifiedName ?? string.Empty
            };

            PlayerPrefs.SetString(OpenTabsPrefsKey, JsonUtility.ToJson(session));
            PlayerPrefs.SetString(ActiveTabPrefsKey, session.activeTab);
            PlayerPrefs.Save();
        }

        public void RestoreTabSession()
        {
            if (!PlayerPrefs.HasKey(OpenTabsPrefsKey))
                return;

            var json = PlayerPrefs.GetString(OpenTabsPrefsKey, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
                return;

            var session = JsonUtility.FromJson<TabSessionData>(json);
            if (session?.openTabs == null || session.openTabs.Length == 0)
                return;

            var availableTypes = new HashSet<Type>(GameDataTypeCache.Types);

            foreach (var typeName in session.openTabs)
            {
                var type = ResolveType(typeName);
                if (type == null || !availableTypes.Contains(type))
                    continue;

                OpenTab(type);
            }

            var activeName = string.IsNullOrWhiteSpace(session.activeTab)
                ? PlayerPrefs.GetString(ActiveTabPrefsKey, string.Empty)
                : session.activeTab;
            var activeType = ResolveType(activeName);
            if (activeType != null && _tabs.ContainsKey(activeType))
                ActivateTab(activeType);
        }

        private static Type ResolveType(string assemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(assemblyQualifiedName))
                return null;

            return Type.GetType(assemblyQualifiedName, false);
        }

        [Serializable]
        private sealed class TabSessionData
        {
            public string[] openTabs;
            public string activeTab;
        }

        private sealed class TabState
        {
            public DataEditor View { get; }
            public VisualElement Tab { get; }

            public TabState(DataEditor view, VisualElement tab)
            {
                View = view;
                Tab = tab;
            }
        }
    }
}
#endif