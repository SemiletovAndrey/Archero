using System;
using System.Collections.Concurrent;
using System.Reflection;
using Newtonsoft.Json;
using Template.Scripts.Attributes;
using Template.Scripts.Enums;

namespace Template.Scripts.Converters
{
    public class EnumConverter : JsonConverter
    {
        private static readonly ConcurrentDictionary<Type, SerializableEnumAttribute> AttributeCache = new();

        public override bool CanConvert(Type objectType)
        {
            Type enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return enumType.IsEnum && GetAttribute(enumType) != null;
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            Type enumType = value.GetType();

            var attr = GetAttribute(enumType);

            if (attr == null)
            {
                serializer.Serialize(writer, value);
                return;
            }

            switch (attr.SerializationType)
            {
                case EnumSerializationType.String:
                    writer.WriteValue(EnumMemberCacheWrapper.EnumToString(enumType, value));
                    break;

                case EnumSerializationType.Int:
                    writer.WriteValue(Convert.ToInt32(value));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            Type enumType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            string key = reader.Value!.ToString();
            
            if (int.TryParse(key, out int i))
                return Enum.ToObject(enumType, i);

            if (EnumMemberCacheWrapper.TryStringToEnum(enumType, key, out var result))
                return result;

            throw new Exception($"Invalid enum key: {key}");
        }

        private static SerializableEnumAttribute GetAttribute(Type enumType)
        {
            return AttributeCache.GetOrAdd(
                enumType,
                static t => t.GetCustomAttribute<SerializableEnumAttribute>());
        }
    }
}