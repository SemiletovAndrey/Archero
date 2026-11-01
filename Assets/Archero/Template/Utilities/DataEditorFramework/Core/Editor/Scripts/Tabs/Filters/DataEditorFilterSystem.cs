#if  UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    internal sealed class DataEditorFilterSystem
    {
        private readonly List<FilterCondition> _filterConditions = new();
        private readonly DataEditorFilterEvaluation _evaluation = new();

        private VisualElement _filterPanel;
        private VisualElement _conditionsHost;
        private ToolbarMenu _filterModeMenu;
        private Toggle _wholeConfigToggle;
        private ColorField _filterColorField;

        private FilterEffectMode _filterMode = FilterEffectMode.Hide;
        private Color _filterColor = new(1f, 0.45f, 0.45f, 0.25f);
        private string _searchText = string.Empty;
        private Action _onChanged;
        private Func<IReadOnlyList<FieldDescriptor>> _descriptorsProvider;
        private IList _lastItems;
        private bool _isRebindingValueField;
        private string _descriptorSignature = string.Empty;
        private bool _menusDirty = true;

        public VisualElement BuildPanel(Func<IReadOnlyList<FieldDescriptor>> descriptorsProvider, Action onChanged)
        {
            _descriptorsProvider = descriptorsProvider;
            _onChanged = onChanged;

            _filterPanel = new VisualElement();
            _filterPanel.AddToClassList("filter-panel");

            var toolbar = new VisualElement();
            toolbar.AddToClassList("filter-toolbar");

            var addConditionBtn = new Button(AddCondition) { text = "Add Condition" };

            _filterModeMenu = new ToolbarMenu();
            _filterModeMenu.AddToClassList("filter-mode-menu");
            RebuildFilterModeMenu();

            _wholeConfigToggle = new Toggle("Whole Config");
            _wholeConfigToggle.SetValueWithoutNotify(true);
            _wholeConfigToggle.RegisterValueChangedCallback(_ => _onChanged?.Invoke());

            _filterColorField = new ColorField("Highlight");
            _filterColorField.value = _filterColor;
            _filterColorField.RegisterValueChangedCallback(evt =>
            {
                _filterColor = evt.newValue;
                _onChanged?.Invoke();
            });

            toolbar.Add(addConditionBtn);
            toolbar.Add(_filterModeMenu);
            toolbar.Add(_wholeConfigToggle);
            toolbar.Add(_filterColorField);

            _conditionsHost = new VisualElement();
            _conditionsHost.AddToClassList("filter-conditions");

            _filterPanel.Add(toolbar);
            _filterPanel.Add(_conditionsHost);
            return _filterPanel;
        }

        public void TogglePanel()
        {
            if (_filterPanel == null)
                return;

            _filterPanel.style.display = _filterPanel.style.display == DisplayStyle.None
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        public void SetSearchText(string searchText)
        {
            _searchText = searchText ?? string.Empty;
        }

        public void RefreshConditionMenus(IReadOnlyList<FieldDescriptor> descriptors)
        {
            var signature = BuildDescriptorSignature(descriptors);
            if (!_menusDirty && signature == _descriptorSignature)
                return;

            foreach (var condition in _filterConditions)
                RefreshConditionMenus(condition, descriptors);

            _descriptorSignature = signature;
            _menusDirty = false;
        }

        public void Evaluate(IList sourceItems, List<object> visibleItems, IReadOnlyList<FieldDescriptor> descriptors)
        {
            _lastItems = sourceItems;
            DataEditorFilterEvaluator.EvaluateVisibleItems(
                sourceItems,
                visibleItems,
                descriptors,
                _filterConditions,
                _searchText,
                _filterMode,
                _wholeConfigToggle.value,
                _evaluation);
        }

        public void ApplyListFieldVisuals(object item, string fieldKey, VisualElement element)
        {
            if (!HasActiveConditions() || _wholeConfigToggle.value)
                return;

            if (!_evaluation.ItemFailedFields.TryGetValue(item, out var failedFields) || !failedFields.Contains(fieldKey))
                return;

            if (_filterMode == FilterEffectMode.Hide)
                element.style.display = DisplayStyle.None;
            else
                element.style.backgroundColor = _filterColor;
        }

        public void ApplyListItemVisuals(object item, VisualElement element)
        {
            if (!HasActiveConditions())
                return;

            if (_filterMode == FilterEffectMode.Paint && _wholeConfigToggle.value && _evaluation.MismatchedItems.Contains(item))
                element.style.backgroundColor = _filterColor;
        }

        public void ApplyTableCellVisuals(object item, string fieldKey, VisualElement element)
        {
            if (!HasActiveConditions())
                return;

            if (_wholeConfigToggle.value)
            {
                if (_filterMode == FilterEffectMode.Paint && _evaluation.MismatchedItems.Contains(item))
                    element.style.backgroundColor = _filterColor;
                return;
            }

            if (!_evaluation.ItemFailedFields.TryGetValue(item, out var failedFields))
                return;

            if (_filterMode == FilterEffectMode.Hide)
            {
                if (fieldKey != null && failedFields.Contains(fieldKey))
                    element.style.display = DisplayStyle.None;
            }
            else if (fieldKey != null && failedFields.Contains(fieldKey))
            {
                element.style.backgroundColor = _filterColor;
            }
        }

        private void AddCondition()
        {
            var condition = new FilterCondition();
            _filterConditions.Add(condition);
            _menusDirty = true;
            AddConditionRow(condition);
            _onChanged?.Invoke();
        }

        private void AddConditionRow(FilterCondition condition)
        {
            var row = new VisualElement();
            row.AddToClassList("filter-condition-row");

            condition.Row = row;
            condition.FieldMenu = new ToolbarMenu { text = "Field" };
            condition.FieldMenu.AddToClassList("filter-field-menu");
            condition.OperatorMenu = new ToolbarMenu { text = "==" };
            condition.OperatorMenu.AddToClassList("filter-op-menu");
            condition.ValueHost = new VisualElement();
            condition.ValueHost.AddToClassList("filter-value-field");

            var removeBtn = new Button(() =>
            {
                DisposeConditionCarrier(condition);
                _filterConditions.Remove(condition);
                _menusDirty = true;
                row.RemoveFromHierarchy();
                _onChanged?.Invoke();
            })
            { text = "x" };
            removeBtn.AddToClassList("filter-remove-button");

            row.Add(condition.FieldMenu);
            row.Add(condition.OperatorMenu);
            row.Add(condition.ValueHost);
            row.Add(removeBtn);
            _conditionsHost.Add(row);

            RefreshConditionMenus(condition, _descriptorsProvider?.Invoke() ?? Array.Empty<FieldDescriptor>());
        }

        private void RefreshConditionMenus(FilterCondition condition, IReadOnlyList<FieldDescriptor> descriptors)
        {
            condition.FieldMenu.menu.ClearItems();

            foreach (var descriptor in descriptors)
            {
                var localDescriptor = descriptor;
                condition.FieldMenu.menu.AppendAction(localDescriptor.DisplayName, _ =>
                {
                    condition.FieldKey = localDescriptor.Key;
                    condition.FieldMenu.text = localDescriptor.DisplayName;
                    EnsureConditionValueBinding(condition, localDescriptor);
                    if (!DataEditorFilterEvaluator.IsOperatorAllowedForField(localDescriptor.Field.FieldType, condition.Operator))
                        condition.Operator = FilterOperator.Equal;
                    RefreshOperatorMenu(condition, localDescriptor.Field.FieldType);
                    _onChanged?.Invoke();
                });
            }

            var selected = FieldDescriptorLookup.GetByKey(condition.FieldKey, descriptors);
            condition.FieldMenu.text = selected?.DisplayName ?? "Field";
            if (selected.HasValue)
                EnsureConditionValueBinding(condition, selected.Value);
            RefreshOperatorMenu(condition, selected?.Field.FieldType);
        }

        private void RefreshOperatorMenu(FilterCondition condition, Type fieldType)
        {
            condition.OperatorMenu.menu.ClearItems();
            var operators = DataEditorFilterEvaluator.GetAllowedOperators(fieldType);

            foreach (var op in operators)
            {
                var localOp = op;
                condition.OperatorMenu.menu.AppendAction(DataEditorFilterEvaluator.GetOperatorLabel(localOp), _ =>
                {
                    condition.Operator = localOp;
                    condition.OperatorMenu.text = DataEditorFilterEvaluator.GetOperatorLabel(localOp);
                    _onChanged?.Invoke();
                });
            }

            if (!operators.Contains(condition.Operator))
                condition.Operator = operators[0];

            condition.OperatorMenu.text = DataEditorFilterEvaluator.GetOperatorLabel(condition.Operator);
        }

        private void EnsureConditionValueBinding(FilterCondition condition, FieldDescriptor descriptor)
        {
            var carrierType = GetCarrierTypeForDescriptor(descriptor);
            if (carrierType == null)
            {
                condition.ValueHost.Clear();
                return;
            }

            var targetPropertyPath = descriptor.Field.Name;
            if (condition.ValueCarrier != null &&
                condition.ValueCarrier.GetType() == carrierType &&
                condition.ActiveValuePropertyPath == targetPropertyPath &&
                condition.ValueHost.childCount > 0)
                return;

            if (condition.ValueCarrier == null || condition.ValueCarrier.GetType() != carrierType)
            {
                DisposeConditionCarrier(condition);
                condition.ValueCarrier = ScriptableObject.CreateInstance(carrierType);
            }

            var serializedObject = new SerializedObject(condition.ValueCarrier);
            var valueProperty = serializedObject.FindProperty(targetPropertyPath);
            if (valueProperty == null)
            {
                condition.ValueHost.Clear();
                return;
            }

            condition.ActiveValuePropertyPath = valueProperty.propertyPath;
            condition.ValueHost.Unbind();
            condition.ValueHost.Clear();

            var propertyField = new PropertyField(valueProperty.Copy()) { label = string.Empty };
            _isRebindingValueField = true;
            propertyField.Bind(serializedObject);
            _isRebindingValueField = false;
            propertyField.RegisterCallback<SerializedPropertyChangeEvent>(_ =>
            {
                if (_isRebindingValueField)
                    return;
                serializedObject.ApplyModifiedProperties();
                _onChanged?.Invoke();
            });

            condition.ValueHost.Add(propertyField);
        }

        private Type GetCarrierTypeForDescriptor(FieldDescriptor descriptor)
        {
            var declared = descriptor.Field.DeclaringType;
            if (declared != null && typeof(ScriptableObject).IsAssignableFrom(declared))
                return declared;

            if (_lastItems != null)
            {
                foreach (var item in _lastItems)
                {
                    if (item == null)
                        continue;

                    var itemType = item.GetType();
                    if (typeof(ScriptableObject).IsAssignableFrom(itemType) &&
                        descriptor.Field.DeclaringType != null &&
                        descriptor.Field.DeclaringType.IsAssignableFrom(itemType))
                        return itemType;
                }
            }

            return null;
        }

        private bool HasActiveConditions()
        {
            for (var i = 0; i < _filterConditions.Count; i++)
            {
                if (DataEditorFilterEvaluator.IsConditionActive(_filterConditions[i]))
                    return true;
            }

            return false;
        }

        private static string BuildDescriptorSignature(IReadOnlyList<FieldDescriptor> descriptors)
        {
            if (descriptors == null || descriptors.Count == 0)
                return string.Empty;

            return string.Join("|", descriptors.Select(d => d.Key));
        }

        private void RebuildFilterModeMenu()
        {
            _filterModeMenu.menu.ClearItems();
            _filterModeMenu.text = _filterMode == FilterEffectMode.Hide ? "Hide Non-Match" : "Paint Non-Match";

            _filterModeMenu.menu.AppendAction("Hide Non-Match", _ =>
            {
                _filterMode = FilterEffectMode.Hide;
                RebuildFilterModeMenu();
                _onChanged?.Invoke();
            });

            _filterModeMenu.menu.AppendAction("Paint Non-Match", _ =>
            {
                _filterMode = FilterEffectMode.Paint;
                RebuildFilterModeMenu();
                _onChanged?.Invoke();
            });
        }

        private static void DisposeConditionCarrier(FilterCondition condition)
        {
            if (condition.ValueCarrier != null)
                UnityEngine.Object.DestroyImmediate(condition.ValueCarrier);
            condition.ValueCarrier = null;
            condition.ActiveValuePropertyPath = null;
        }
    }
}
#endif