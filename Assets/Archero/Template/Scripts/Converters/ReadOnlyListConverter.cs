using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Template.Scripts.Converters
{
    public class ReadOnlyListConverter<T> : JsonConverter<IReadOnlyList<T>>
    {
        public override void WriteJson(JsonWriter writer, IReadOnlyList<T> value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override IReadOnlyList<T> ReadJson(
            JsonReader reader,
            Type objectType,
            IReadOnlyList<T> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var list = serializer.Deserialize<List<T>>(reader);
            return list;
        }
    }
    
}