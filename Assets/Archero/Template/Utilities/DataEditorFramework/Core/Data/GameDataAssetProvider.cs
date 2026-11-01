#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DataEditorFramework.Core.Data
{
    public static class GameDataAssetProvider
    {
        private const string SEARCH_PATH = "Assets/";

        private static readonly Dictionary<Type, List<ScriptableObject>> _cache = new();

        public static IReadOnlyList<ScriptableObject> GetAssets(Type type)
        {
            if (!_cache.TryGetValue(type, out var list))
            {
                list = LoadAssets(type);
                _cache[type] = list;
            }

            return list;
        }

        private static List<ScriptableObject> LoadAssets(Type type)
        {
            var result = new List<ScriptableObject>();

            var filter = $"t:{type.Name}";
            var guids = AssetDatabase.FindAssets(filter, new[] { SEARCH_PATH });

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(path, type) as ScriptableObject;

                if (asset != null)
                    result.Add(asset);
            }

            return result;
        }

        public static void Refresh()
        {
            _cache.Clear();
        }
    }
}
#endif
