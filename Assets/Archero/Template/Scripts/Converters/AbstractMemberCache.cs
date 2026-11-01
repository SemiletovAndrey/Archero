using System;
using System.Collections.Generic;
using System.Reflection;
using Template.Scripts.Attributes;

namespace Template.Scripts.Converters
{
    public class AbstractMemberCache
    {
        private static readonly Dictionary<(Type enumType, long value), Type> _enumMap = new();
        private static readonly Dictionary<string, Type> _stringMap = new();

        static AbstractMemberCache()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in type.GetTypes())
                {
                    IEnumerable<AbstractMemberAttribute> attrs = t.GetCustomAttributes<AbstractMemberAttribute>();
                    foreach (AbstractMemberAttribute attr in attrs)
                    {
                        if (attr.StringValue == null)
                        {
                            _enumMap[(attr.EnumType, attr.EnumValue)] = t;
                        }
                        else
                        {
                            _stringMap[attr.StringValue] = t;
                        }
                    }
                }
            }
        }
        
        public static Type Get(Type enumType, long value)
        {
            return _enumMap.TryGetValue((enumType, value), out var t)
                ? t
                : throw new Exception($"No mapping for {enumType}:{value}");
        }
        
        public static Type Get(string value)
        {
            return _stringMap.TryGetValue(value, out var t)
                ? t
                : throw new Exception($"No mapping for {value}");
        }
        
        public static bool TryGet(Type enumType, long value, out Type result)
        {
            return _enumMap.TryGetValue((enumType, value), out result);
        }
        
        public static bool TryGet(string value, out Type result)
        {
            return _stringMap.TryGetValue(value, out result);
        }
    }
}