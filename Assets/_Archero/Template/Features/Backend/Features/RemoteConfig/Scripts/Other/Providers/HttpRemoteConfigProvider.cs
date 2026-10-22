using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Template.Features.Config.Remote;
using Template.Scripts.DataHolders;
using Template.Scripts.Helpers;
using UnityEngine;

namespace Template.Features.Backend.Http
{
    public class HttpRemoteConfigProvider : BaseHttpHandler, IRemoteConfigProvider
    {
        private const string LatestVersionRoute = "latest";
        private const string VersionParameter = "version";
        private const string ConfigParameter = "config";

        private Dictionary<string, JToken> _jsonBalance = new();
        private Dictionary<string, object> _objectBalance = new();

        public int Priority => 0;
        protected override string Route => "config";

        public async UniTask InitAsync()
        {
            await DownloadLatestConfig();
        }

        public bool TryGetConfig<T>(string key, out T config) where T : class, IRemoteFeatureConfig
        {
            if (_objectBalance.TryGetValue(key, out object objectValue))
            {
                config = objectValue as T;

                if (config is null)
                {
                    Debug.LogError($"{key} can't be converted to type {typeof(T)}");
                    return false;
                }

                return true;
            }

            if (!_jsonBalance.TryGetValue(key, out JToken jsonValue))
            {
                config = null;
                return false;
            }

            T convertedValue = jsonValue.Deserialize<T>();
            _objectBalance[key] = convertedValue;

            config = convertedValue;
            return true;
        }

        public async UniTask<Result> DownloadLatestConfig()
        {
            Result<string> versionResult = await GetLatestVersion();

            if (versionResult.IsFailure)
            {
                return versionResult;
            }

            return await DownloadConfig(versionResult.Value);
        }

        public async UniTask<Result> DownloadConfig(string version)
        {
            var balanceResult = await LoadConfig(version);
            if (balanceResult.IsFailure)
            {
                return balanceResult;
            }

            _jsonBalance = balanceResult.Value;
            _objectBalance.Clear();

            return Result.Success();
        }

        private async UniTask<Result<string>> GetLatestVersion()
        {
            Result<VersionDTO> result = await CreateGetRequest<VersionDTO>(LatestVersionRoute).Send();

            if (result.IsFailure)
            {
                return Result.Failure<string>(result.Error);
            }

            return Result.Success(result.Value.Version);
        }

        private async UniTask<Result<Dictionary<string, JToken>>> LoadConfig(string version)
        {
            Result<Dictionary<string, JToken>> result = await CreateGetRequest<Dictionary<string, JToken>>()
                .WithParameters(new HttpRequestParameterBuilder()
                    .AddParameter(VersionParameter, version))
                .Send();

            if (result.IsFailure)
            {
                return Result.Failure<Dictionary<string, JToken>>(result.Error);
            }

            return Result.Success(result.Value);
        }

        public class ConfigResultDTO
        {
            [JsonProperty(ConfigParameter)] public Dictionary<string, JToken> Config;
        }

        public class VersionDTO
        {
            [JsonProperty(VersionParameter)] public string Version;
        }
    }
}