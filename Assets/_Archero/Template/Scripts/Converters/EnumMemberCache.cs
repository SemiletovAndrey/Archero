using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Template.Scripts.Attributes;
using UnityEngine;

namespace Template.Scripts.Converters
{
    public static class EnumMemberCacheWrapper
    {
        private static readonly ConcurrentDictionary<Type, ICache> _cache = new();

        public static bool TryStringToEnum(Type enumType, string key, out object value)
        {
            var cache = _cache.GetOrAdd(enumType, CreateCache);
            return cache.TryParse(key, out value);
        }
        
        public static object StringToEnum(Type enumType, string key)
        {
            var cache = _cache.GetOrAdd(enumType, CreateCache);

            if (cache.TryParse(key, out object value))
            {
                return value;
            }

            throw new ArgumentException($"Enum type {enumType} with key {key} is not found");
        }

        public static string EnumToString(Type enumType, object value)
        {
            var cache = _cache.GetOrAdd(enumType, CreateCache);
            return cache.EnumToString(value);
        }

        private static ICache CreateCache(Type enumType)
        {
            Type cacheType = typeof(Cache<>).MakeGenericType(enumType);
            return (ICache)Activator.CreateInstance(cacheType)!;
        }

        private interface ICache
        {
            public bool TryParse(string key, out object value);
            public string EnumToString(object key);
        }

        private sealed class Cache<T> : ICache where T : struct, Enum
        {
            private static readonly Dictionary<string, T> _fromString;
            private static readonly Dictionary<T, string> _toStringMap;

            static Cache()
            {
                _fromString = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
                _toStringMap = new Dictionary<T, string>();

                foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var value = (T)field.GetValue(null);

                    var attr = field.GetCustomAttribute<EnumMemberAttribute>();
                    string name = attr?.Value ?? field.Name;

                    _fromString[name] = value;
                    _toStringMap[value] = name;

                    foreach (var formerName in field.GetCustomAttributes<FormerNameAttribute>())
                    {
                        _fromString[formerName.Value] = value;
                    }
                }
            }

            public static string EnumToString(T value)
            {
                return _toStringMap.TryGetValue(value, out var str)
                    ? str
                    : value.ToString();
            }

            public static bool TryParse(string key, out T value)
            {
                return _fromString.TryGetValue(key, out value);
            }

            public bool TryParse(string key, out object value)
            {
                if (!TryParse(key, out T genericValue))
                {
                    value = null;
                    return false;
                }
                
                value = genericValue;
                return true;
            }

            public string EnumToString(object key)
            {
                return EnumToString((T)key);
            }
        }
    }
}