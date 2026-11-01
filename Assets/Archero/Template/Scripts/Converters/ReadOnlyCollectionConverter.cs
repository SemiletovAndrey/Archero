using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Template.Scripts.Converters;

namespace Nova.Domain.Scripts.Converters
{
    public class ReadOnlyCollectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (!objectType.IsGenericType)
                return false;

            Type def = objectType.GetGenericTypeDefinition();

            return def == typeof(IReadOnlyList<>) ||
                   def == typeof(IReadOnlyDictionary<,>);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            Type[] args = objectType.GetGenericArguments();
            Type genericType = objectType.GetGenericTypeDefinition();

            JsonConverter converter = CreateConverter(genericType, args);

            return converter.ReadJson(
                reader,
                objectType,
                existingValue,
                serializer);
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
        
        private static JsonConverter CreateConverter(
            Type genericType,
            Type[] args)
        {
            Type converterType;

            if (genericType == typeof(IReadOnlyList<>))
            {
                converterType = typeof(ReadOnlyListConverter<>)
                    .MakeGenericType(args);
            }
            else if (genericType == typeof(IReadOnlyDictionary<,>))
            {
                converterType = typeof(ReadOnlyDictionaryConverter<,>)
                    .MakeGenericType(args);
            }
            else
            {
                throw new NotSupportedException(genericType.ToString());
            }

            return (JsonConverter)Activator.CreateInstance(converterType)!;
        }
    }
}