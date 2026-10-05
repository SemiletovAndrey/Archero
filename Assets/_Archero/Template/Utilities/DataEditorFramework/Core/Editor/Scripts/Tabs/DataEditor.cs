#if  UNITY_EDITOR
using System;
using System.Collections.Generic;
using DataEditorFramework.Core.Data;
using DataEditorFramework.Core.Editor.Scripts.DataController;
using DataEditorFramework.Core.Editor.Scripts.Enums;
using DataEditorFramework.Core.Editor.Scripts.Rendering.DataRenderers;
using DataEditorFramework.Core.Editor.Scripts.Rendering.DataRenderers.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    public class DataEditor
    {
        public VisualElement Root { get; } = new();

        private readonly Type _type;
        private readonly IDataRenderer _dataRenderer;
        private readonly IDataController _assetDataController;
        private readonly VisualElement _bodyHost = new();
        private readonly List<object> _visibleItems = new();
        private readonly List<SortColumnDescription> _tableSorting = new();

        private readonly DataEditorFieldVisibilitySystem _fieldVisibilitySystem = new();
        private readonly DataEditorFilterSystem _filterSystem = new();

        private BaseVerticalCollectionView _collectionView;
        private VisualElement _detailPane;
        private object _selectedItem;
        private ConfigEntryViewMode _viewMode;
        private ToolbarMenu _viewModeMenu;
        private TextField _searchField;
        private Button _deleteButton;
        private object _renamingItem;
        

        public DataEditor(Type type, ConfigEntryViewMode initialViewMode = ConfigEntryViewMode.Inspector)
        {
            _type = type;
            _dataRenderer = DataRendererRegistry.GetRenderer(_type);
            _assetDataController = DataControllerFactory.Create(type);
            _viewMode = initialViewMode;

            Root.AddToClassList("data-editor-view");
            Root.Add(BuildSearchAndActionsRow());

            var filterPanel = _filterSystem.BuildPanel(
                _fieldVisibilitySystem.GetAllDescriptors,
                RefreshAndRebuild);

            filterPanel.style.display = DisplayStyle.None;
            Root.Add(filterPanel);
            Root.Add(_bodyHost);

            RefreshAndRebuild();
        }

        public void SavePendingChanges()
        {
            _assetDataController.SaveChanges();
        }

        private VisualElement BuildSearchAndActionsRow()
        {
            var row = new VisualElement();
            row.AddToClassList("search-actions-row");

            _searchField = new TextField();
            _searchField.AddToClassList("search-field");
            _searchField.RegisterValueChangedCallback(evt =>
            {
                _filterSystem.SetSearchText(evt.newValue);
                RefreshAndRebuild();
            });

            var actions = new VisualElement();
            actions.AddToClassList("action-buttons");

            var addBtn = new Button(() =>
            {
                _assetDataController.AddNew(GetCurrentTypeDirectory());
                RefreshAndRebuild();
            })
            {
                text = "Add",
                style = { display = _type.IsAbstract ? DisplayStyle.None : DisplayStyle.Flex }
            };

            _deleteButton = new Button(DeleteSelected) { text = "Delete" };

            var importBtn = new Button(() =>
            {
                _assetDataController.ImportJson();
                RefreshAndRebuild();
            }) { text = "Import" };

            var exportBtn = new Button(() => _assetDataController.ExportJson()) { text = "Export" };
            var filterBtn = new Button(_filterSystem.TogglePanel) { text = "Filter" };

            actions.Add(BuildViewModeMenu());
            actions.Add(_fieldVisibilitySystem.BuildMenu(RebuildBody));
            actions.Add(filterBtn);
            actions.Add(addBtn);
            actions.Add(_deleteButton);
            actions.Add(importBtn);
            actions.Add(exportBtn);

            row.Add(_searchField);
            row.Add(actions);
            return row;
        }

        private ToolbarMenu BuildViewModeMenu()
        {
            _viewModeMenu = new ToolbarMenu();
            _viewModeMenu.AddToClassList("view-mode-menu");
            UpdateViewModeMenuText();

            foreach (ConfigEntryViewMode mode in Enum.GetValues(typeof(ConfigEntryViewMode)))
            {
                var localMode = mode;
                _viewModeMenu.menu.AppendAction(localMode.ToString(), _ => SetViewMode(localMode),
                    _ => _viewMode == localMode ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            }

            return _viewModeMenu;
        }

        private void UpdateViewModeMenuText()
        {
            if (_viewModeMenu != null)
                _viewModeMenu.text = _viewMode.ToString();
        }

        private void SetViewMode(ConfigEntryViewMode viewMode)
        {
            if (_viewMode == viewMode)
                return;

            _viewMode = viewMode;
            UpdateViewModeMenuText();
            RebuildBody();
        }

        private void RefreshAndRebuild()
        {
            CaptureTableSorting();
            _fieldVisibilitySystem.RebuildDescriptors(_assetDataController.Items, _dataRenderer, _type);
            _filterSystem.RefreshConditionMenus(_fieldVisibilitySystem.GetAllDescriptors());
            _filterSystem.Evaluate(_assetDataController.Items, _visibleItems, _fieldVisibilitySystem.GetAllDescriptors());
            RebuildBody();
        }

        private void RebuildBody()
        {
            _bodyHost.Clear();
            _bodyHost.AddToClassList("editor-body-host");

            if (_viewMode == ConfigEntryViewMode.Table)
            {
                var split = new TwoPaneSplitView(1, 380, TwoPaneSplitViewOrientation.Horizontal);
                split.AddToClassList("editor-split-view");

                _collectionView = CreateTableView();
                _collectionView.AddToClassList("editor-split-table");

                _detailPane = new ScrollView();
                _detailPane.AddToClassList("editor-detail-pane");

                split.Add(_collectionView);
                split.Add(_detailPane);
                _bodyHost.Add(split);

                if (_selectedItem == null || !_visibleItems.Contains(_selectedItem))
                    _selectedItem = GetFirstVisibleItem();

                if (_collectionView is MultiColumnListView table && _selectedItem != null)
                {
                    var selectedIndex = _visibleItems.IndexOf(_selectedItem);
                    if (selectedIndex >= 0)
                        table.SetSelection(selectedIndex);
                }

                RebuildDetailPane();
            }
            else
            {
                _collectionView = CreateListView();
                _bodyHost.Add(_collectionView);
            }

            UpdateDeleteButtonState();
        }

        private ListView CreateListView()
        {
            var list = new ListView
            {
                fixedItemHeight = 100,
                itemsSource = _visibleItems,
                makeItem = () =>
                {
                    var item = new VisualElement();
                    item.AddToClassList("editor-body-item");
                    return item;
                },
                bindItem = BindListItem,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };

            list.selectionChanged += OnListSelectionChanged;
            list.AddToClassList("editor-body");
            _collectionView = list;
            return list;
        }

        private MultiColumnListView CreateTableView()
        {
            var table = new MultiColumnListView
            {
                itemsSource = _visibleItems,
                selectionType = SelectionType.Single,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };

            table.AddToClassList("editor-body");
            table.showBorder = true;
            table.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            table.sortingMode = ColumnSortingMode.Default;
            table.selectionChanged += OnTableSelectionChanged;

            table.columns.Add(new Column
            {
                title = "Config Name",
                width = 220,
                minWidth = 160,
                stretchable = true,
                sortable = true,
                comparison = CompareConfigNameRows,
                makeCell = MakeConfigNameCell,
                bindCell = (element, rowIndex) =>
                {
                    if (!TryGetVisibleItem(rowIndex, out var item))
                    {
                        element.style.display = DisplayStyle.None;
                        return;
                    }

                    BindConfigNameCell(element, item);
                }
            });

            foreach (var descriptor in _fieldVisibilitySystem.GetVisibleDescriptors())
            {
                var localDescriptor = descriptor;
                var canSort = DataSortRuleRegistry.TryGetRule(localDescriptor.Field, out _);
                table.columns.Add(new Column
                {
                    title = localDescriptor.DisplayName,
                    minWidth = 120,
                    stretchable = true,
                    sortable = canSort,
                    comparison = canSort ? (leftIndex, rightIndex) => CompareFieldRows(leftIndex, rightIndex, localDescriptor) : null,
                    makeCell = () => new VisualElement(),
                    bindCell = (element, index) => BindTableCell(element, index, localDescriptor)
                });
            }

            ApplyTableSorting(table);
            _collectionView = table;
            return table;
        }

        private int CompareConfigNameRows(int leftIndex, int rightIndex)
        {
            if (!TryGetVisibleItem(leftIndex, out var leftItem) || !TryGetVisibleItem(rightIndex, out var rightItem))
                return 0;

            return DataSortValueComparer.Compare(
                (leftItem as UnityEngine.Object)?.name,
                (rightItem as UnityEngine.Object)?.name);
        }

        private int CompareFieldRows(int leftIndex, int rightIndex, FieldDescriptor descriptor)
        {
            if (!DataSortRuleRegistry.TryGetRule(descriptor.Field, out var rule))
                return 0;

            try
            {
                if (!TryGetVisibleItem(leftIndex, out var leftItem) || !TryGetVisibleItem(rightIndex, out var rightItem))
                    return 0;

                return rule.Compare(leftItem, rightItem, descriptor.Field);
            }
            catch
            {
                return 0;
            }
        }

        private void BindListItem(VisualElement element, int index)
        {
            if (!TryGetVisibleItem(index, out var item))
            {
                element.Clear();
                element.style.display = DisplayStyle.None;
                return;
            }

            element.Clear();
            element.style.backgroundColor = StyleKeyword.Null;
            element.style.display = DisplayStyle.Flex;

            var label = new Label(((UnityEngine.Object)item).name);
            label.AddToClassList("editor-body-item-label");
            label.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0 || evt.clickCount != 2)
                    return;

                BeginRename(item);
                evt.StopPropagation();
            });
            element.Add(label);

            var renameField = new TextField();
            renameField.AddToClassList("editor-body-item-label");
            renameField.style.display = IsRenaming(item) ? DisplayStyle.Flex : DisplayStyle.None;
            renameField.value = ((UnityEngine.Object)item).name;
            renameField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.KeypadEnter)
                    return;

                CommitRename(item, renameField.value);
                evt.StopPropagation();
            });
            renameField.RegisterCallback<FocusOutEvent>(_ => CommitRename(item, renameField.value));
            element.Add(renameField);

            label.style.display = IsRenaming(item) ? DisplayStyle.None : DisplayStyle.Flex;
            if (IsRenaming(item))
            {
                renameField.schedule.Execute(() =>
                {
                    renameField.Focus();
                    renameField.SelectAll();
                });
            }

            foreach (var descriptor in _fieldVisibilitySystem.GetVisibleDescriptors())
            {
                var propertyField = _dataRenderer.RenderField(item, descriptor.Field, _ => _assetDataController.MarkDirty(item));
                if (propertyField == null)
                    continue;

                propertyField.label = descriptor.DisplayName;
                propertyField.style.display = DisplayStyle.Flex;
                propertyField.style.backgroundColor = StyleKeyword.Null;
                _filterSystem.ApplyListFieldVisuals(item, descriptor.Key, propertyField);
                element.Add(propertyField);
            }

            _filterSystem.ApplyListItemVisuals(item, element);
        }

        private void BindTableCell(VisualElement element, int index, FieldDescriptor descriptor)
        {
            if (!TryGetVisibleItem(index, out var item))
            {
                element.Clear();
                element.style.display = DisplayStyle.None;
                return;
            }

            element.Clear();
            element.style.backgroundColor = StyleKeyword.Null;
            element.style.display = DisplayStyle.Flex;

            if (FieldSummaryFormatter.ShouldUseSummary(descriptor.Field))
            {
                var summary = new Label(FieldSummaryFormatter.BuildText(item, descriptor.Field));
                summary.AddToClassList("editor-table-summary");
                _filterSystem.ApplyTableCellVisuals(item, descriptor.Key, summary);
                element.Add(summary);
            }
            else
            {
                var propertyField = _dataRenderer.RenderField(item, descriptor.Field, _ => _assetDataController.MarkDirty(item));
                if (propertyField != null)
                {
                    _filterSystem.ApplyTableCellVisuals(item, descriptor.Key, propertyField);
                    element.Add(propertyField);
                }
                else
                {
                    _filterSystem.ApplyTableCellVisuals(item, descriptor.Key, element);
                }
            }
        }

        private void OnListSelectionChanged(IEnumerable<object> selected)
        {
            foreach (var item in selected)
            {
                _selectedItem = item;
                break;
            }

            UpdateDeleteButtonState();
        }

        private void OnTableSelectionChanged(IEnumerable<object> selected)
        {
            foreach (var item in selected)
            {
                _selectedItem = item;
                break;
            }

            RebuildDetailPane();
            UpdateDeleteButtonState();
        }

        private void RebuildDetailPane()
        {
            if (_detailPane == null)
                return;

            _detailPane.Clear();

            if (_selectedItem == null)
            {
                _detailPane.Add(new Label("No Selection"));
                return;
            }

            var selectedName = (_selectedItem as UnityEngine.Object)?.name ?? _selectedItem.GetType().Name;
            var header = new Label(selectedName);
            header.AddToClassList("editor-detail-header");
            _detailPane.Add(header);

            foreach (var descriptor in _fieldVisibilitySystem.GetAllDescriptors())
            {
                if (descriptor.Field.DeclaringType != null &&
                    !descriptor.Field.DeclaringType.IsInstanceOfType(_selectedItem))
                    continue;

                var propertyField = _dataRenderer.RenderField(_selectedItem, descriptor.Field,
                    _ => _assetDataController.MarkDirty(_selectedItem));
                if (propertyField == null)
                    continue;

                propertyField.label = descriptor.DisplayName;
                _detailPane.Add(propertyField);
            }
        }

        private void DeleteSelected()
        {
            if (_selectedItem == null)
                return;

            _assetDataController.Remove(_selectedItem);
            _selectedItem = null;
            RefreshAndRebuild();
        }

        private void UpdateDeleteButtonState()
        {
            _deleteButton?.SetEnabled(_selectedItem != null);
        }

        private VisualElement MakeConfigNameCell()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;

            var label = new Label { name = "config-name-label" };
            label.style.flexGrow = 1;
            label.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0 || evt.clickCount != 2)
                    return;

                if (label.userData != null)
                    BeginRename(label.userData);
                evt.StopPropagation();
            });

            var renameField = new TextField { name = "config-name-rename" };
            renameField.style.flexGrow = 1;
            renameField.style.display = DisplayStyle.None;
            renameField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode != KeyCode.Return && evt.keyCode != KeyCode.KeypadEnter)
                    return;

                if (renameField.userData != null)
                    CommitRename(renameField.userData, renameField.value);
                evt.StopPropagation();
            });
            renameField.RegisterCallback<FocusOutEvent>(_ =>
            {
                if (renameField.userData != null)
                    CommitRename(renameField.userData, renameField.value);
            });

            root.Add(label);
            root.Add(renameField);
            return root;
        }

        private void BindConfigNameCell(VisualElement root, object item)
        {
            var label = root.Q<Label>("config-name-label");
            var renameField = root.Q<TextField>("config-name-rename");
            if (label == null || renameField == null)
                return;

            var name = ((UnityEngine.Object)item).name;
            label.userData = item;
            renameField.userData = item;
            label.text = name;
            renameField.SetValueWithoutNotify(name);

            var isRenaming = IsRenaming(item);
            label.style.display = isRenaming ? DisplayStyle.None : DisplayStyle.Flex;
            renameField.style.display = isRenaming ? DisplayStyle.Flex : DisplayStyle.None;
            if (isRenaming)
            {
                renameField.schedule.Execute(() =>
                {
                    renameField.Focus();
                    renameField.SelectAll();
                });
            }

            _filterSystem.ApplyTableCellVisuals(item, null, isRenaming ? renameField : label);
        }

        private void BeginRename(object item)
        {
            if (item == null)
                return;

            _renamingItem = item;
            RefreshRenameVisuals();
        }

        private void CommitRename(object item, string enteredName)
        {
            if (!ReferenceEquals(_renamingItem, item))
                return;

            _renamingItem = null;
            var trimmedName = (enteredName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                RefreshRenameVisuals();
                return;
            }

            if (item is not UnityEngine.Object unityObject)
            {
                RefreshRenameVisuals();
                return;
            }

            if (unityObject.name == trimmedName)
            {
                RefreshRenameVisuals();
                return;
            }

            var assetPath = AssetDatabase.GetAssetPath(unityObject);
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                RefreshRenameVisuals();
                return;
            }

            var renameError = AssetDatabase.RenameAsset(assetPath, trimmedName);
            if (!string.IsNullOrWhiteSpace(renameError))
                Debug.LogError($"Failed to rename asset '{assetPath}' to '{trimmedName}': {renameError}");

            AssetDatabase.SaveAssets();
            GameDataAssetProvider.Refresh();
            RefreshAndRebuild();
        }

        private bool IsRenaming(object item)
        {
            return ReferenceEquals(_renamingItem, item);
        }

        private void RefreshRenameVisuals()
        {
            _collectionView?.RefreshItems();
            if (_viewMode == ConfigEntryViewMode.Table)
                RebuildDetailPane();
        }

        private string GetCurrentTypeDirectory()
        {
            if (_assetDataController.Items.Count <= 0)
                return null;

            if (_assetDataController.Items[0] is not UnityEngine.Object firstItem)
                return null;

            var path = AssetDatabase.GetAssetPath(firstItem);
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var lastSlash = path.LastIndexOf('/');
            return lastSlash <= 0 ? null : path.Substring(0, lastSlash);
        }

        private void CaptureTableSorting()
        {
            if (_collectionView is not MultiColumnListView table)
                return;

            _tableSorting.Clear();
            foreach (var sortDescription in table.sortColumnDescriptions)
                _tableSorting.Add(sortDescription);
        }

        private void ApplyTableSorting(MultiColumnListView table)
        {
            if (_tableSorting.Count == 0)
                return;

            table.sortColumnDescriptions.Clear();
            foreach (var sortDescription in _tableSorting)
                table.sortColumnDescriptions.Add(sortDescription);
        }

        private bool TryGetVisibleItem(int index, out object item)
        {
            item = null;
            if (index < 0 || index >= _visibleItems.Count)
                return false;

            item = _visibleItems[index];
            return item != null;
        }

        private object GetFirstVisibleItem()
        {
            for (var i = 0; i < _visibleItems.Count; i++)
            {
                if (_visibleItems[i] != null)
                    return _visibleItems[i];
            }

            return null;
        }

    }
}
#endif