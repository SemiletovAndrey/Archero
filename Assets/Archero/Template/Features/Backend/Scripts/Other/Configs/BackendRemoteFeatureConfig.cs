using Newtonsoft.Json;
using Template.Features.Config.Remote;

namespace Template.Features.Backend
{
    public class BackendRemoteFeatureConfig : IRemoteFeatureConfig
    {
        public const string Key = "backend";
        
        [JsonProperty("http_url")] public string HttpUrl { get; private set; }
        [JsonProperty("socket_url")] public string SocketUrl { get; private set; }
    }
}