using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataEditorFramework.Core.Data.Attribute;

namespace DataEditorFramework.Core.Data
{
    public static class DataSortRuleRegistry
    {
        private static Dictionary<Type, List<IDataSortRule>> _cache;
        private static readonly List<IDataSortRule> DefaultRules = new()
        {
            new UnityObjectDataSortRule(),
            new CollectionCountDataSortRule(),
            new ComparableDataSortRule()
        };

        static DataSortRuleRegistry()
        {
            BuildCache();
        }

        public static bool TryGetRule(FieldInfo field, out IDataSortRule rule)
        {
            foreach (var customRule in GetCustomRules(field.FieldType))
            {
                if (customRule.CanSort(field))
                {
                    rule = customRule;
                    return true;
                }
            }

            foreach (var defaultRule in DefaultRules)
            {
                if (defaultRule.CanSort(field))
                {
                    rule = defaultRule;
                    return true;
                }
            }

            rule = null;
            return false;
        }

        private static void BuildCache()
        {
            _cache = new Dictionary<Type, List<IDataSortRule>>();

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetLoadableTypes))
            {
                if (type == null || type.IsAbstract || type.IsInterface)
                    continue;

                if (!typeof(IDataSortRule).IsAssignableFrom(type))
                    continue;

                var attributes = type.GetCustomAttributes<DataSortRuleAttribute>().ToArray();
                if (attributes.Length == 0)
                    continue;

                foreach (var attribute in attributes)
                {
                    if (attribute.TargetType == null)
                        continue;

                    if (!_cache.TryGetValue(attribute.TargetType, out var rules))
                    {
                        rules = new List<IDataSortRule>();
                        _cache[attribute.TargetType] = rules;
                    }

                    if (Activator.CreateInstance(type) is IDataSortRule rule)
                        rules.Add(rule);
                }
            }
        }

        private static IEnumerable<IDataSortRule> GetCustomRules(Type fieldType)
        {
            if (fieldType == null)
                yield break;

            if (_cache.TryGetValue(fieldType, out var exactRules))
            {
                foreach (var rule in exactRules)
                    yield return rule;
            }

            foreach (var pair in _cache)
            {
                if (pair.Key == fieldType || !pair.Key.IsAssignableFrom(fieldType))
                    continue;

                foreach (var rule in pair.Value)
                    yield return rule;
            }
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
