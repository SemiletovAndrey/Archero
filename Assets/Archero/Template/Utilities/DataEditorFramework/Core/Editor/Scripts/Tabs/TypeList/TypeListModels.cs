using System;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    internal enum TypeListEntryKind
    {
        None,
        Type,
        Folder
    }

    internal readonly struct TypeListEntry
    {
        public readonly TypeListEntryKind Kind;
        public readonly Type Type;
        public readonly string FolderId;
        public readonly string DisplayName;
        public readonly int Depth;
        public readonly bool HasChildren;

        private TypeListEntry(TypeListEntryKind kind, Type type, string folderId, string displayName, int depth, bool hasChildren)
        {
            Kind = kind;
            Type = type;
            FolderId = folderId;
            DisplayName = displayName;
            Depth = depth;
            HasChildren = hasChildren;
        }

        public static TypeListEntry ForType(Type type, int depth, bool hasChildren)
        {
            return new TypeListEntry(TypeListEntryKind.Type, type, null, type.Name, depth, hasChildren);
        }

        public static TypeListEntry ForFolder(string folderId, string name, int depth, bool hasChildren)
        {
            return new TypeListEntry(TypeListEntryKind.Folder, null, folderId, name, depth, hasChildren);
        }
    }

    [Serializable]
    internal sealed class TypeListFolderState
    {
        public string Id;
        public string Name;
        public string ParentId;
    }

    [Serializable]
    internal sealed class TypeListParentState
    {
        public string TypeName;
        public string FolderId;
    }

    [Serializable]
    internal sealed class TypeListFolderStateData
    {
        public TypeListFolderState[] folders;
    }

    [Serializable]
    internal sealed class TypeListParentStateData
    {
        public TypeListParentState[] parents;
    }
}
