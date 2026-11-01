using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nova.Domain.Scripts.Converters;
using Template.Scripts.Converters;

namespace Template.Scripts.Helpers
{
    public static class JsonHelpers
    {
        private static JsonSerializerSettings _settings;
        private static readonly JsonSerializer _serializer;

        static JsonHelpers()
        {
            _settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new ReadOnlyCollectionConverter(),
                    new EnumConverter()
                }
            };
            
            _serializer = JsonSerializer.Create(_settings);
        }
        
        public static string Serialize(object value, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(value, formatting, _settings);
        }
        
        public static T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _settings);
        }

        public static T Deserialize<T>(this JToken jToken)
        {
            return jToken.ToObject<T>(_serializer);
        }
    }
}