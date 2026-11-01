#if  UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using DataEditorFramework.Core.Data.Attribute;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    internal sealed class TypeListState
    {
        private readonly TypeListStateStore _store;
        private readonly List<TypeListFolderState> _folders = new();
        private readonly Dictionary<string, string> _typeParents = new();

        public TypeListState(TypeListStateStore store)
        {
            _store = store;
            _store.Load(_folders, _typeParents);
        }

        public List<TypeListEntry> BuildVisibleEntries(
            ICollection<Type> expandedTypes,
            ICollection<string> expandedFolders)
        {
            var visibleEntries = new List<TypeListEntry>();
            AddFoldersRecursive(null, 0, expandedTypes, expandedFolders, visibleEntries);

            foreach (var rootType in GameDataTypeCache.RootTypes)
            {
                if (GetTypeParent(rootType) == null)
                    AddTypeRecursive(rootType, 0, expandedTypes, visibleEntries);
            }

            return visibleEntries;
        }

        public List<TypeListEntry> BuildFilteredEntries(string query)
        {
            var normalizedQuery = (query ?? string.Empty).Trim();
            var visibleEntries = new List<TypeListEntry>();

            AddFilteredFoldersRecursive(null, 0, normalizedQuery, visibleEntries);

            foreach (var rootType in GameDataTypeCache.RootTypes)
            {
                if (GetTypeParent(rootType) != null)
                    continue;

                AddTypeRecursiveFiltered(rootType, 0, normalizedQuery, visibleEntries);
            }

            return visibleEntries;
        }

        public string GetTypeParent(Type type)
        {
            return _typeParents.TryGetValue(GetTypeKey(type), out var parentId) && FolderExists(parentId)
                ? parentId
                : null;
        }

        public string CreateFolder(string parentFolderId)
        {
            var parent = NormalizeParent(parentFolderId);
            var folder = new TypeListFolderState
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = CreateUniqueFolderName(parent),
                ParentId = parent
            };

            _folders.Add(folder);
            Save();
            return folder.Id;
        }

        public void RenameFolder(string folderId, string name)
        {
            var folder = _folders.FirstOrDefault(f => f.Id == folderId);
            if (folder == null)
                return;

            folder.Name = string.IsNullOrWhiteSpace(name) ? "New Folder" : name.Trim();
            Save();
        }

        public void DeleteFolder(string folderId)
        {
            var folder = _folders.FirstOrDefault(f => f.Id == folderId);
            if (folder == null)
                return;

            var parentId = folder.ParentId;
            foreach (var child in _folders.Where(f => f.ParentId == folderId))
                child.ParentId = parentId;

            foreach (var typeKey in _typeParents.Where(pair => pair.Value == folderId).Select(pair => pair.Key).ToArray())
            {
                if (parentId == null)
                    _typeParents.Remove(typeKey);
                else
                    _typeParents[typeKey] = parentId;
            }

            _folders.Remove(folder);
            Save();
        }

        public bool CanDropOn(TypeListEntry dragged, TypeListEntry target)
        {
            if (target.Kind != TypeListEntryKind.Folder || dragged.Kind == TypeListEntryKind.None)
                return false;

            if (dragged.Kind == TypeListEntryKind.Folder)
                return dragged.FolderId != target.FolderId && !IsDescendantFolder(target.FolderId, dragged.FolderId);

            return GetTypeParent(dragged.Type) != target.FolderId;
        }

        public bool MoveEntryToFolder(TypeListEntry entry, string folderId)
        {
            folderId = NormalizeParent(folderId);

            if (entry.Kind == TypeListEntryKind.Folder)
            {
                var folder = _folders.FirstOrDefault(f => f.Id == entry.FolderId);
                if (folder == null || folder.Id == folderId || IsDescendantFolder(folderId, folder.Id))
                    return false;

                folder.ParentId = folderId;
            }
            else if (entry.Kind == TypeListEntryKind.Type)
            {
                var key = GetTypeKey(entry.Type);
                if (folderId == null)
                    _typeParents.Remove(key);
                else
                    _typeParents[key] = folderId;
            }
            else
            {
                return false;
            }

            Save();
            return true;
        }

        private void AddFoldersRecursive(
            string parentFolderId,
            int depth,
            ICollection<Type> expandedTypes,
            ICollection<string> expandedFolders,
            List<TypeListEntry> visibleEntries)
        {
            foreach (var folder in GetChildFolders(parentFolderId))
            {
                var hasChildren = GetChildFolders(folder.Id).Any() || GetFolderTypes(folder.Id).Any();
                visibleEntries.Add(TypeListEntry.ForFolder(folder.Id, folder.Name, depth, hasChildren));

                if (!expandedFolders.Contains(folder.Id))
                    continue;

                AddFoldersRecursive(folder.Id, depth + 1, expandedTypes, expandedFolders, visibleEntries);
                foreach (var type in GetFolderTypes(folder.Id))
                    AddTypeRecursive(type, depth + 1, expandedTypes, visibleEntries);
            }
        }

        private void AddTypeRecursive(Type type, int depth, ICollection<Type> expandedTypes, List<TypeListEntry> visibleEntries)
        {
            var children = GameDataTypeCache.GetDirectChildren(type)
                .Where(child => GetTypeParent(child) == null)
                .ToList();

            var hasChildren = children.Count > 0;
            visibleEntries.Add(TypeListEntry.ForType(type, depth, hasChildren));

            if (!hasChildren || !expandedTypes.Contains(type))
                return;

            foreach (var child in children)
                AddTypeRecursive(child, depth + 1, expandedTypes, visibleEntries);
        }

        private bool AddFilteredFoldersRecursive(
            string parentFolderId,
            int depth,
            string query,
            List<TypeListEntry> visibleEntries)
        {
            var anyMatch = false;
            foreach (var folder in GetChildFolders(parentFolderId))
            {
                var folderMatches = false;
                var branchEntries = new List<TypeListEntry>();

                if (AddFilteredFoldersRecursive(folder.Id, depth + 1, query, branchEntries))
                    folderMatches = true;

                foreach (var type in GetFolderTypes(folder.Id))
                {
                    if (AddTypeRecursiveFiltered(type, depth + 1, query, branchEntries))
                        folderMatches = true;
                }

                if (!folderMatches)
                    continue;

                visibleEntries.Add(TypeListEntry.ForFolder(folder.Id, folder.Name, depth, false));
                visibleEntries.AddRange(branchEntries);
                anyMatch = true;
            }

            return anyMatch;
        }

        private bool AddTypeRecursiveFiltered(
            Type type,
            int depth,
            string query,
            List<TypeListEntry> visibleEntries)
        {
            var branchEntries = new List<TypeListEntry>();
            var hasMatchingChild = false;

            foreach (var child in GameDataTypeCache.GetDirectChildren(type).Where(t => GetTypeParent(t) == null))
            {
                if (AddTypeRecursiveFiltered(child, depth + 1, query, branchEntries))
                    hasMatchingChild = true;
            }

            var selfMatches = TypeMatches(type, query);
            if (!selfMatches && !hasMatchingChild)
                return false;

            visibleEntries.Add(TypeListEntry.ForType(type, depth, false));
            visibleEntries.AddRange(branchEntries);
            return true;
        }

        private static bool TypeMatches(Type type, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return true;

            return type.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private IEnumerable<TypeListFolderState> GetChildFolders(string parentFolderId)
        {
            return _folders
                .Where(f => string.Equals(NormalizeParent(f.ParentId), NormalizeParent(parentFolderId), StringComparison.Ordinal))
                .OrderBy(f => f.Name);
        }

        private IEnumerable<Type> GetFolderTypes(string folderId)
        {
            return GameDataTypeCache.Types
                .Where(type => string.Equals(GetTypeParent(type), folderId, StringComparison.Ordinal))
                .OrderBy(type => type.Name);
        }

        private string CreateUniqueFolderName(string parentFolderId)
        {
            const string baseName = "New Folder";
            var siblings = GetChildFolders(parentFolderId).Select(f => f.Name).ToHashSet();
            if (!siblings.Contains(baseName))
                return baseName;

            var index = 2;
            while (siblings.Contains($"{baseName} {index}"))
                index++;

            return $"{baseName} {index}";
        }

        private bool FolderExists(string folderId)
        {
            return folderId != null && _folders.Any(f => f.Id == folderId);
        }

        private bool IsDescendantFolder(string folderId, string possibleAncestorId)
        {
            folderId = NormalizeParent(folderId);
            while (folderId != null)
            {
                if (folderId == possibleAncestorId)
                    return true;

                folderId = _folders.FirstOrDefault(f => f.Id == folderId)?.ParentId;
            }

            return false;
        }

        private void Save()
        {
            _store.Save(_folders, _typeParents);
        }

        private static string NormalizeParent(string parentFolderId)
        {
            return string.IsNullOrWhiteSpace(parentFolderId) ? null : parentFolderId;
        }

        private static string GetTypeKey(Type type)
        {
            return type.AssemblyQualifiedName;
        }
    }
}
#endif