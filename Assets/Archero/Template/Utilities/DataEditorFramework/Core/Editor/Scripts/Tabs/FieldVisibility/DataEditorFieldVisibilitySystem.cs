#if  UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataEditorFramework.Core.Editor.Scripts.Extensions;
using DataEditorFramework.Core.Editor.Scripts.Rendering.DataRenderers;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    internal sealed class DataEditorFieldVisibilitySystem
    {
        private readonly List<FieldDescriptor> _fieldDescriptors = new();
        private readonly Dictionary<string, bool> _fieldVisibility = new();

        private ToolbarMenu _fieldVisibilityMenu;
        private Action _onChanged;

        public ToolbarMenu BuildMenu(Action onChanged)
        {
            _onChanged = onChanged;
            _fieldVisibilityMenu = new ToolbarMenu { text = "Fields" };
            _fieldVisibilityMenu.AddToClassList("field-visibility-menu");
            RebuildMenuItems();
            return _fieldVisibilityMenu;
        }

        public void RebuildDescriptors(IList sourceItems, IDataRenderer dataRenderer, Type fallbackType)
        {
            _fieldDescriptors.Clear();
            var knownKeys = new HashSet<string>();

            foreach (var scanType in GetTypesForItems(sourceItems, fallbackType))
            {
                foreach (var field in dataRenderer.GetEditableFields(scanType))
                {
                    var key = $"{field.DeclaringType?.FullName ?? scanType.FullName}.{field.Name}";
                    if (!knownKeys.Add(key))
                        continue;

                    _fieldDescriptors.Add(new FieldDescriptor(key, field.GetDisplayName(), field));
                    if (!_fieldVisibility.ContainsKey(key))
                        _fieldVisibility[key] = true;
                }
            }

            _fieldDescriptors.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.Ordinal));
            RebuildMenuItems();
        }

        public IEnumerable<FieldDescriptor> GetVisibleDescriptors()
        {
            return _fieldDescriptors.Where(descriptor => _fieldVisibility.GetValueOrDefault(descriptor.Key, true));
        }

        public IReadOnlyList<FieldDescriptor> GetAllDescriptors()
        {
            return _fieldDescriptors;
        }

        public FieldDescriptor? GetByKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            foreach (var descriptor in _fieldDescriptors)
            {
                if (descriptor.Key == key)
                    return descriptor;
            }

            return null;
        }

        private void RebuildMenuItems()
        {
            if (_fieldVisibilityMenu == null)
                return;

            _fieldVisibilityMenu.menu.ClearItems();
            _fieldVisibilityMenu.menu.AppendAction("Show All", _ =>
            {
                foreach (var descriptor in _fieldDescriptors)
                    _fieldVisibility[descriptor.Key] = true;
                _onChanged?.Invoke();
            });

            _fieldVisibilityMenu.menu.AppendAction("Hide All", _ =>
            {
                foreach (var descriptor in _fieldDescriptors)
                    _fieldVisibility[descriptor.Key] = false;
                _onChanged?.Invoke();
            });

            _fieldVisibilityMenu.menu.AppendSeparator();

            foreach (var descriptor in _fieldDescriptors)
            {
                var localDescriptor = descriptor;
                _fieldVisibilityMenu.menu.AppendAction(localDescriptor.DisplayName, _ =>
                    {
                        _fieldVisibility[localDescriptor.Key] = !_fieldVisibility[localDescriptor.Key];
                        _onChanged?.Invoke();
                    },
                    _ => _fieldVisibility.GetValueOrDefault(localDescriptor.Key, true)
                        ? DropdownMenuAction.Status.Checked
                        : DropdownMenuAction.Status.Normal);
            }
        }

        private static IEnumerable<Type> GetTypesForItems(IList items, Type fallbackType)
        {
            var runtimeTypes = new HashSet<Type>();
            foreach (var item in items)
            {
                if (item != null)
                    runtimeTypes.Add(item.GetType());
            }

            if (runtimeTypes.Count == 0)
                runtimeTypes.Add(fallbackType);

            return runtimeTypes;
        }
    }
}
#endif