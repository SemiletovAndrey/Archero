using System.Diagnostics;
using System.IO;
using Unity.CodeEditor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Template.PlatformUtils
{
    public static class OpenInRider
    {
        private const string MenuPath = "Tools/Open Last File in Rider %&r";
    private const string PrefsKey = "OpenInRider_LastFilePath";

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnSelectionChanged()
    {
        Object activeObject = Selection.activeObject;
        if (activeObject == null) return;

        string assetPath = AssetDatabase.GetAssetPath(activeObject);
        if (string.IsNullOrEmpty(assetPath)) return;

        if (IsSupportedFile(assetPath))
        {
            string fullPath = Path.GetFullPath(assetPath);
            if (File.Exists(fullPath))
            {
                // Сохраняем в EditorPrefs, чтобы путь не терялся после перекомпиляции скриптов
                EditorPrefs.SetString(PrefsKey, fullPath);
            }
        }
    }

    [MenuItem(MenuPath)]
    private static void OpenLastFile()
    {
        string targetPath = EditorPrefs.GetString(PrefsKey, string.Empty);
        
        if (string.IsNullOrEmpty(targetPath) || !File.Exists(targetPath))
        {
            string currentAsset = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(currentAsset) && IsSupportedFile(currentAsset))
            {
                targetPath = Path.GetFullPath(currentAsset);
            }
        }

        if (string.IsNullOrEmpty(targetPath) || !File.Exists(targetPath))
        {
            Debug.LogWarning("OpenInRider: Нет сохраненного скрипта. Кликните на любой .cs файл в окне Project.");
            return;
        }

        CodeEditor.CurrentEditor.SyncAll();

        bool opened = CodeEditor.CurrentEditor.OpenProject(targetPath, 0, 0);

        if (!opened)
        {
            Debug.LogError($"OpenInRider: Не удалось открыть {targetPath}. Проверьте, что Rider выбран в Edit -> Preferences -> External Tools.");
        }
    }

    private static bool IsSupportedFile(string path)
    {
        return path.EndsWith(".cs") ||
               path.EndsWith(".shader") ||
               path.EndsWith(".hlsl") ||
               path.EndsWith(".asmdef") ||
               path.EndsWith(".json");
    }
    }
}