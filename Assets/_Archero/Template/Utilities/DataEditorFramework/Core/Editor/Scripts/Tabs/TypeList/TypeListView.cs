#if  UNITY_EDITOR
using System;
using System.Collections.Generic;
using DataEditorFramework.Core.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    public class TypeListView : VisualElement
    {
        private const string DragDataKey = "DataEditorFramework.TypeList.DragEntry";

        private readonly TypeListState _state;
        private readonly TextField _searchField;
        private readonly ListView _listView;
        private readonly List<TypeListEntry> _visibleEntries = new();
        private readonly HashSet<Type> _expandedTypes = new();
        private readonly HashSet<string> _expandedFolders = new();
        private readonly Action<Type> _onSelected;

        private TypeListEntry _pressedEntry;
        private Vector2 _pressedPosition;
        private bool _dragStarted;
        private string _editingFolderId;
        private string _searchQuery;

        public TypeListView(Action<Type> onSelected)
        {
            _onSelected = onSelected;
            _state = new TypeListState(new TypeListStateStore());

            AddToClassList("type-list");
            _searchField = new TextField
            {
                name = "type-search-field"
            };
            _searchField.AddToClassList("type-search-field");
            _searchField.RegisterValueChangedCallback(evt =>
            {
                _searchQuery = evt.newValue;
                RebuildList();
            });
            Add(_searchField);

            BuildVisibleEntries();

            _listView = new ListView
            {
                itemsSource = _visibleEntries,
                selectionType = SelectionType.Single,
                makeItem = MakeItem,
                bindItem = BindItem,
                style = { flexGrow = 1 }
            };

            _listView.selectionChanged += OnSelectionChanged;
            _listView.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Create Folder", _ => CreateFolder(null));
            }));
            _listView.RegisterCallback<DragUpdatedEvent>(OnRootDragUpdated);
            _listView.RegisterCallback<DragPerformEvent>(OnRootDragPerform);

            Add(_listView);
        }

        private VisualElement MakeItem()
        {
            var container = new VisualElement();
            container.AddToClassList("type-item");

            var treePrefix = new Label();
            treePrefix.name = "type-tree-prefix";
            treePrefix.AddToClassList("type-tree-prefix");
            container.Add(treePrefix);

            var foldToggle = new Button();
            foldToggle.name = "type-fold-toggle";
            foldToggle.AddToClassList("type-fold-toggle");
            foldToggle.clicked += () =>
            {
                if (foldToggle.userData is TypeListEntry entry)
                    ToggleEntry(entry);
            };
            container.Add(foldToggle);

            var label = new Label();
            label.name = "type-label";
            label.AddToClassList("type-label");

            var renameField = new TextField();
            renameField.name = "type-folder-rename";
            renameField.AddToClassList("type-folder-rename");
            renameField.style.display = DisplayStyle.None;
            renameField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    CommitFolderRename(renameField);
                    evt.StopPropagation();
                }
            });
            renameField.RegisterCallback<FocusOutEvent>(_ => CommitFolderRename(renameField));

            var count = new Label();
            count.name = "type-count";
            count.AddToClassList("type-count");

            container.Add(label);
            container.Add(renameField);
            container.Add(count);

            container.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                if (container.userData is not TypeListEntry entry)
                    return;

                if (entry.Kind == TypeListEntryKind.Folder)
                {
                    evt.menu.AppendAction("Create Folder", _ => CreateFolder(entry.FolderId));
                    evt.menu.AppendAction("Rename", _ => BeginFolderRename(entry.FolderId));
                    evt.menu.AppendAction("Delete Folder", _ => DeleteFolder(entry.FolderId));
                }
                else
                {
                    evt.menu.AppendAction("Create Folder", _ => CreateFolder(null));
                }

                evt.StopPropagation();
            }));

            container.RegisterCallback<MouseDownEvent>(OnItemMouseDown);
            container.RegisterCallback<MouseMoveEvent>(OnItemMouseMove);
            container.RegisterCallback<MouseUpEvent>(OnItemMouseUp);
            container.RegisterCallback<DragUpdatedEvent>(OnItemDragUpdated);
            container.RegisterCallback<DragPerformEvent>(OnItemDragPerform);

            return container;
        }

        private void BindItem(VisualElement element, int index)
        {
            var entry = _visibleEntries[index];
            element.userData = entry;
            element.EnableInClassList("type-folder-item", entry.Kind == TypeListEntryKind.Folder);

            var foldToggle = element.Q<Button>("type-fold-toggle");
            var treePrefix = element.Q<Label>("type-tree-prefix");
            var label = element.Q<Label>("type-label");
            var renameField = element.Q<TextField>("type-folder-rename");
            var countLabel = element.Q<Label>("type-count");

            treePrefix.text = BuildTreePrefix(index);
            /*if (entry.Depth > 1)
            {
                treePrefix.style.paddingLeft = new StyleLength(20f);
            }*/
            label.text = entry.DisplayName;
            renameField.style.marginLeft = 0;
            renameField.userData = entry.FolderId;
            renameField.value = entry.DisplayName;

            var isEditing = entry.Kind == TypeListEntryKind.Folder && entry.FolderId == _editingFolderId;
            label.style.display = isEditing ? DisplayStyle.None : DisplayStyle.Flex;
            renameField.style.display = isEditing ? DisplayStyle.Flex : DisplayStyle.None;

            if (isEditing)
                renameField.schedule.Execute(() =>
                {
                    renameField.Focus();
                    renameField.SelectAll();
                });

            if (entry.Kind == TypeListEntryKind.Type)
            {
                countLabel.style.display = DisplayStyle.Flex;
                countLabel.text = GameDataAssetProvider.GetAssets(entry.Type).Count.ToString();
            }
            else
            {
                countLabel.style.display = DisplayStyle.None;
                countLabel.text = string.Empty;
            }

            foldToggle.userData = entry;

            if (entry.HasChildren)
            {
                foldToggle.style.display = DisplayStyle.Flex;
                foldToggle.style.visibility = Visibility.Visible;
                foldToggle.text = IsExpanded(entry) ? "v" : ">";
            }
            else
            {
                foldToggle.style.display = DisplayStyle.Flex;
                foldToggle.style.visibility = Visibility.Hidden;
                foldToggle.text = string.Empty;
            }
        }

        private void OnSelectionChanged(IEnumerable<object> selection)
        {
            foreach (var obj in selection)
            {
                if (obj is TypeListEntry { Kind: TypeListEntryKind.Type } entry)
                {
                    _onSelected?.Invoke(entry.Type);
                    break;
                }
            }
        }

        private void ToggleEntry(TypeListEntry entry)
        {
            if (entry.Kind == TypeListEntryKind.Folder)
            {
                if (!_expandedFolders.Add(entry.FolderId))
                    _expandedFolders.Remove(entry.FolderId);
            }
            else
            {
                if (!_expandedTypes.Add(entry.Type))
                    _expandedTypes.Remove(entry.Type);
            }

            RebuildList();
        }

        private bool IsExpanded(TypeListEntry entry)
        {
            return entry.Kind == TypeListEntryKind.Folder
                ? _expandedFolders.Contains(entry.FolderId)
                : _expandedTypes.Contains(entry.Type);
        }

        private void BuildVisibleEntries()
        {
            _visibleEntries.Clear();
            if (string.IsNullOrWhiteSpace(_searchQuery))
                _visibleEntries.AddRange(_state.BuildVisibleEntries(_expandedTypes, _expandedFolders));
            else
                _visibleEntries.AddRange(_state.BuildFilteredEntries(_searchQuery));
        }

        private void CreateFolder(string parentFolderId)
        {
            var folderId = _state.CreateFolder(parentFolderId);
            if (!string.IsNullOrWhiteSpace(parentFolderId))
                _expandedFolders.Add(parentFolderId);

            _editingFolderId = folderId;
            RebuildList();
        }

        private void BeginFolderRename(string folderId)
        {
            _editingFolderId = folderId;
            RebuildList();
        }

        private void CommitFolderRename(TextField field)
        {
            if (field.userData is not string folderId || _editingFolderId != folderId)
                return;

            _state.RenameFolder(folderId, field.value);

            _editingFolderId = null;
            RebuildList();
        }

        private void DeleteFolder(string folderId)
        {
            _state.DeleteFolder(folderId);
            _expandedFolders.Remove(folderId);
            RebuildList();
        }

        private void OnItemMouseDown(MouseDownEvent evt)
        {
            if (evt.currentTarget is not VisualElement element || element.userData is not TypeListEntry entry)
                return;

            if (evt.button == 0 && evt.clickCount == 2 && entry.Kind == TypeListEntryKind.Folder)
            {
                BeginFolderRename(entry.FolderId);
                evt.StopPropagation();
                return;
            }

            if (evt.button != 0)
                return;

            _pressedEntry = entry;
            _pressedPosition = evt.mousePosition;
            _dragStarted = false;
        }

        private void OnItemMouseMove(MouseMoveEvent evt)
        {
            if (_pressedEntry.Kind == TypeListEntryKind.None || _dragStarted)
                return;

            if ((evt.mousePosition - _pressedPosition).sqrMagnitude < 16)
                return;

            _dragStarted = true;
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData(DragDataKey, _pressedEntry);
            DragAndDrop.StartDrag(_pressedEntry.DisplayName);
            evt.StopPropagation();
        }

        private void OnItemMouseUp(MouseUpEvent evt)
        {
            _pressedEntry = default;
            _dragStarted = false;
        }

        private void OnItemDragUpdated(DragUpdatedEvent evt)
        {
            if (evt.currentTarget is not VisualElement element || element.userData is not TypeListEntry target)
                return;

            if (CanDropOn(target))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                evt.StopPropagation();
            }
        }

        private void OnItemDragPerform(DragPerformEvent evt)
        {
            if (evt.currentTarget is not VisualElement element || element.userData is not TypeListEntry target)
                return;

            if (!CanDropOn(target))
                return;

            var dragged = GetDraggedEntry();
            MoveEntryToFolder(dragged, target.FolderId);
            DragAndDrop.AcceptDrag();
            evt.StopPropagation();
        }

        private void OnRootDragUpdated(DragUpdatedEvent evt)
        {
            var dragged = GetDraggedEntry();
            if (dragged.Kind == TypeListEntryKind.None)
                return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
        }

        private void OnRootDragPerform(DragPerformEvent evt)
        {
            var dragged = GetDraggedEntry();
            if (dragged.Kind == TypeListEntryKind.None)
                return;

            MoveEntryToFolder(dragged, null);
            DragAndDrop.AcceptDrag();
        }

        private bool CanDropOn(TypeListEntry target)
        {
            return _state.CanDropOn(GetDraggedEntry(), target);
        }

        private TypeListEntry GetDraggedEntry()
        {
            return DragAndDrop.GetGenericData(DragDataKey) is TypeListEntry entry ? entry : default;
        }

        private void MoveEntryToFolder(TypeListEntry entry, string folderId)
        {
            if (!_state.MoveEntryToFolder(entry, folderId))
                return;

            if (!string.IsNullOrWhiteSpace(folderId))
                _expandedFolders.Add(folderId);

            RebuildList();
        }

        private void RebuildList()
        {
            BuildVisibleEntries();
            _listView.Rebuild();
        }

        private string BuildTreePrefix(int index)
        {
            var entry = _visibleEntries[index];
            if (entry.Depth <= 0)
                return string.Empty;

            var prefix = string.Empty;
            for (var ancestorDepth = 0; ancestorDepth <= entry.Depth - 1; ancestorDepth++)
                prefix += HasFollowingSibling(index, ancestorDepth) ? "\u2502\u00A0\u00A0" : "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0";

            prefix += HasFollowingSibling(index, entry.Depth) ? "\u251c\u2500\u00A0" : "\u2514\u2500\u00A0";
            return prefix;
        }

        private bool HasFollowingSibling(int index, int depth)
        {
            for (var i = index + 1; i < _visibleEntries.Count; i++)
            {
                var nextDepth = _visibleEntries[i].Depth;
                if (nextDepth < depth)
                    return false;

                if (nextDepth == depth)
                    return true;
            }

            return false;
        }
    }
}
#endif