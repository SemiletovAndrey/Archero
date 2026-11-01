using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DataEditorFramework.Core.Data.Attribute
{
    public static class GameDataTypeCache
    {
        private static List<Type> _types;
        private static Dictionary<Type, List<Type>> _childrenByType;
        private static List<Type> _rootTypes;

        public static IReadOnlyList<Type> Types
        {
            get
            {
                if (_types == null)
                    Build();

                return _types;
            }
        }

        public static IReadOnlyList<Type> RootTypes
        {
            get
            {
                if (_types == null)
                    Build();

                return _rootTypes;
            }
        }

        public static IReadOnlyList<Type> GetDirectChildren(Type parentType)
        {
            if (_types == null)
                Build();

            if (_childrenByType.TryGetValue(parentType, out var children))
                return children;

            return Array.Empty<Type>();
        }

        private static void Build()
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetLoadableTypes)
                .Distinct()
                .ToList();

            var attributedRoots = allTypes
                .Where(t =>
                    t.GetCustomAttribute<GameDataTypeAttribute>() != null &&
                    typeof(ScriptableObject).IsAssignableFrom(t))
                .ToList();

            _types = allTypes
                .Where(t =>
                    typeof(ScriptableObject).IsAssignableFrom(t) &&
                    attributedRoots.Any(root => root.IsAssignableFrom(t)))
                .OrderBy(t => t.Name)
                .ToList();

            var typeSet = new HashSet<Type>(_types);
            _childrenByType = _types.ToDictionary(_ => _, _ => new List<Type>());
            _rootTypes = new List<Type>();

            foreach (var type in _types)
            {
                var parent = FindNearestParentInSet(type, typeSet);
                if (parent == null)
                {
                    _rootTypes.Add(type);
                    continue;
                }

                _childrenByType[parent].Add(type);
            }

            _rootTypes = _rootTypes.OrderBy(t => t.Name).ToList();
            foreach (var key in _childrenByType.Keys.ToList())
                _childrenByType[key] = _childrenByType[key].OrderBy(t => t.Name).ToList();
        }

        private static Type FindNearestParentInSet(Type type, HashSet<Type> typeSet)
        {
            var parent = type.BaseType;
            while (parent != null && parent != typeof(object))
            {
                if (typeSet.Contains(parent))
                    return parent;

                parent = parent.BaseType;
            }

            return null;
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
