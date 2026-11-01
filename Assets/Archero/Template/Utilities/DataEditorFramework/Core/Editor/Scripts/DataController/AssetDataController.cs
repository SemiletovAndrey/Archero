#if  UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using DataEditorFramework.Core.Data;
using UnityEditor;
using UnityEngine;

namespace DataEditorFramework.Core.Editor.Scripts.DataController
{
    public class AssetDataController : IDataController
    {
        private readonly Type _type;

        public AssetDataController(Type type)
        {
            _type = type;
        }

        public IList Items
            => GameDataAssetProvider.GetAssets(_type).ToList();

        public void AddNew(string preferredDirectory = null)
        {
            var asset = ScriptableObject.CreateInstance(_type);
            var initialDirectory = ResolveInitialDirectory(preferredDirectory);

            var path = EditorUtility.SaveFilePanelInProject(
                "Create Config",
                _type.Name,
                "asset",
                "",
                initialDirectory
            );

            if (string.IsNullOrEmpty(path))
                return;

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            GameDataAssetProvider.Refresh();
        }

        public void Remove(object item)
        {
            var asset = item as ScriptableObject;
            if (asset == null)
                return;

            var path = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();

            GameDataAssetProvider.Refresh();
        }

        public void ImportJson() { }
        public void ExportJson() { }

        public void MarkDirty(object item)
        {
            if (item is UnityEngine.Object unityObject)
                EditorUtility.SetDirty(unityObject);
        }

        public void SaveChanges()
        {
            AssetDatabase.SaveAssets();
        }

        private string ResolveInitialDirectory(string preferredDirectory)
        {
            if (!string.IsNullOrWhiteSpace(preferredDirectory))
                return preferredDirectory;

            var firstAsset = Items.Cast<object>().FirstOrDefault() as UnityEngine.Object;
            if (firstAsset == null)
                return "Assets/Resources/Configs";

            var firstAssetPath = AssetDatabase.GetAssetPath(firstAsset);
            if (string.IsNullOrWhiteSpace(firstAssetPath))
                return "Assets/Resources/Configs";

            var directory = Path.GetDirectoryName(firstAssetPath);
            if (string.IsNullOrWhiteSpace(directory))
                return "Assets/Resources/Configs";

            return directory.Replace("\\", "/");
        }
    }
}
#endif