using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Template.Scripts.Converters;
using Template.Scripts.DataHolders;

namespace Nova.Domain.Scripts.Converters
{
    public class ReadOnlyDictionaryConverter<TKey, TValue> : JsonConverter<IReadOnlyDictionary<TKey, TValue>>
    {
        private readonly KeyMode _mode;

        public ReadOnlyDictionaryConverter()
        {
            if (typeof(TKey).IsEnum)
                _mode = KeyMode.Enum;
            else if (typeof(IStrongId).IsAssignableFrom(typeof(TKey)))
                _mode = KeyMode.StrongId;
            else
                _mode = KeyMode.Default;
        }

        public override IReadOnlyDictionary<TKey, TValue> ReadJson(
            JsonReader reader,
            Type objectType,
            IReadOnlyDictionary<TKey, TValue> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var raw = serializer.Deserialize<Dictionary<string, TValue>>(reader);
            var result = new Dictionary<TKey, TValue>(raw.Count);

            foreach (var kvp in raw)
            {
                result[ConvertKey(kvp.Key)] = kvp.Value;
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, IReadOnlyDictionary<TKey, TValue> value,
            JsonSerializer serializer)
        {
            var dict = new Dictionary<string, TValue>(value.Count);

            foreach (var kvp in value)
            {
                dict[Convert.ToString(kvp.Key)] = kvp.Value;
            }

            serializer.Serialize(writer, dict);
        }

        private TKey ConvertKey(string key)
        {
            switch (_mode)
            {
                case KeyMode.Enum:
                    if (int.TryParse(key, out int i))
                        return (TKey)Enum.ToObject(typeof(TKey), i);

                    if (EnumMemberCacheWrapper.TryStringToEnum(typeof(TKey), key, out var result))
                        return (TKey)result;

                    return default;

                case KeyMode.StrongId:
                    return JsonConvert.DeserializeObject<TKey>($"\"{key}\"");

                default:
                    return (TKey)Convert.ChangeType(key, typeof(TKey));
            }
        }

        private enum KeyMode
        {
            Default,
            Enum,
            StrongId
        }
    }
}