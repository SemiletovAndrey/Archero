using Template.Features.Config.Remote;

namespace Template.Features.Backend.Http
{
    public class RemoteConfigHttpUrlProvider : IHttpUrlProvider
    {
        private readonly IRemoteConfigService _remoteConfigService;

        public RemoteConfigHttpUrlProvider(IRemoteConfigService remoteConfigService)
        {
            _remoteConfigService = remoteConfigService;
        }

        public string CreateUrl(string version, string route)
        {
            return version == null 
                ? BackendUtils.ConcatRoute(GetBaseUrl(), route) 
                : BackendUtils.ConcatRoute(GetBaseUrl(), version, route);
        }

        private string GetBaseUrl()
        {
            //return _remoteConfigService.GetConfig<BackendRemoteFeatureConfig>(BackendRemoteFeatureConfig.Key).HttpUrl;
            return "https://enliveningly-copious-ryan.ngrok-free.dev/api";
        }
    }
}