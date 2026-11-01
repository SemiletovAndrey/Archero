using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Template.Scripts.Attributes;

namespace Template.Scripts.Converters
{
    public class AbstractMembersConverter<TBase> : JsonConverter<IReadOnlyList<TBase>>
        where TBase : class
    {
        public override IReadOnlyList<TBase> ReadJson(
            JsonReader reader,
            Type objectType,
            IReadOnlyList<TBase> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var raw = serializer.Deserialize<IReadOnlyDictionary<string, JToken>>(reader);
            var result = new List<TBase>(raw.Count);

            foreach (var pair in raw)
            {
                Type targetType = AbstractMemberCache.Get(pair.Key);
                object item = pair.Value.ToObject(targetType, serializer)!;

                result.Add((TBase)item);
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, IReadOnlyList<TBase> value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            foreach (var item in value)
            {
                var type = item.GetType();

                var attrs = (AbstractMemberAttribute[])
                    type.GetCustomAttributes(typeof(AbstractMemberAttribute), true);

                if (attrs.Length == 0)
                    continue;

                AbstractMemberAttribute attr = attrs[0];
                string key = attr.StringValue;

                writer.WritePropertyName(key);
                serializer.Serialize(writer, item);
            }

            writer.WriteEndObject();
        }
    }
    
    public class AbstractMembersConverter<TEnum, TBase> : JsonConverter<IReadOnlyList<TBase>>
        where TEnum : struct, Enum
        where TBase : class
    {
        public override IReadOnlyList<TBase> ReadJson(
            JsonReader reader,
            Type objectType,
            IReadOnlyList<TBase> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var raw = serializer.Deserialize<IReadOnlyDictionary<TEnum, JToken>>(reader);
            var result = new List<TBase>(raw.Count);

            foreach (var pair in raw)
            {
                int key = Convert.ToInt32(pair.Key);

                if (!AbstractMemberCache.TryGet(typeof(TEnum), key, out Type targetType))
                {
                    continue;
                }
                
                object item = pair.Value.ToObject(targetType, serializer)!;
                result.Add((TBase)item);
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, IReadOnlyList<TBase> value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            foreach (var item in value)
            {
                var type = item.GetType();

                var attrs = (AbstractMemberAttribute[])
                    type.GetCustomAttributes(typeof(AbstractMemberAttribute), true);

                if (attrs.Length == 0)
                    continue;

                AbstractMemberAttribute attr = attrs[0];
                Type enumType = attr.EnumType;
                
                Type underlyingType = Enum.GetUnderlyingType(enumType);
                object typedValue = Convert.ChangeType(attr.EnumValue, underlyingType);

                string key = EnumMemberCacheWrapper.EnumToString(enumType, typedValue);

                writer.WritePropertyName(key);
                serializer.Serialize(writer, item);
            }

            writer.WriteEndObject();
        }
    }
}