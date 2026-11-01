#if  UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DataEditorFramework.Core.Editor.Scripts.Tabs
{
    internal sealed class TypeListStateStore
    {
        private const string FolderPrefsKey = "DataEditorFramework.TypeList.Folders";
        private const string TypeParentPrefsKey = "DataEditorFramework.TypeList.TypeParents";

        public void Load(List<TypeListFolderState> folders, Dictionary<string, string> typeParents)
        {
            folders.Clear();
            typeParents.Clear();

            var folderJson = EditorPrefs.GetString(FolderPrefsKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(folderJson))
            {
                var data = JsonUtility.FromJson<TypeListFolderStateData>(folderJson);
                if (data?.folders != null)
                    folders.AddRange(data.folders.Where(f => !string.IsNullOrWhiteSpace(f.Id)));
            }

            var parentJson = EditorPrefs.GetString(TypeParentPrefsKey, string.Empty);
            if (string.IsNullOrWhiteSpace(parentJson))
                return;

            var parentData = JsonUtility.FromJson<TypeListParentStateData>(parentJson);
            if (parentData?.parents == null)
                return;

            foreach (var parent in parentData.parents)
            {
                if (!string.IsNullOrWhiteSpace(parent.TypeName) && FolderExists(folders, parent.FolderId))
                    typeParents[parent.TypeName] = parent.FolderId;
            }
        }

        public void Save(IReadOnlyList<TypeListFolderState> folders, IReadOnlyDictionary<string, string> typeParents)
        {
            var folderData = new TypeListFolderStateData { folders = folders.ToArray() };
            var parentData = new TypeListParentStateData
            {
                parents = typeParents
                    .Where(pair => FolderExists(folders, pair.Value))
                    .Select(pair => new TypeListParentState { TypeName = pair.Key, FolderId = pair.Value })
                    .ToArray()
            };

            EditorPrefs.SetString(FolderPrefsKey, JsonUtility.ToJson(folderData));
            EditorPrefs.SetString(TypeParentPrefsKey, JsonUtility.ToJson(parentData));
        }

        private static bool FolderExists(IEnumerable<TypeListFolderState> folders, string folderId)
        {
            return folderId != null && folders.Any(f => f.Id == folderId);
        }
    }
}
#endif