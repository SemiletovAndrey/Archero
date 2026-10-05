using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Template.Features.Log;
using Template.Scripts.Helpers;
using UnityEngine;

namespace Template.Features.Config.Remote
{
    [CreateAssetMenu(fileName = "Config_Remote_Provider_File", menuName = "SO/Template/Config/Remote/FileProvider")]
    public class FileRemoteConfigProvider : BaseRemoteConfigProvider
    {
        [SerializeField] private int _priority;
        [SerializeField] private TextAsset _textFile;
        
        private readonly Dictionary<string, object> _objectBalance = new();
        private Dictionary<string, JToken> _jsonBalance;

        public override int Priority => _priority;

        public override void Init()
        {
            _jsonBalance = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(_textFile.text);
        }

        public override bool TryGetConfig<TConfig>(string key, out TConfig config)
        {
            _jsonBalance ??= JsonHelpers.Deserialize<Dictionary<string, JToken>>(_textFile.text);
            
            if (_objectBalance.TryGetValue(key, out object objectValue))
            {
                config = objectValue as TConfig;

                if (config is null)
                {
                    DebugLogger.LogError($"{key} can't be converted to type {typeof(TConfig)}");
                    return false;
                }

                return true;
            }

            if (!_jsonBalance.TryGetValue(key, out JToken jsonValue))
            {
                config = null;
                return false;
            }

            TConfig convertedValue = jsonValue.Deserialize<TConfig>();
            _objectBalance[key] = convertedValue;

            config = convertedValue;
            return true;
        }
    }
}