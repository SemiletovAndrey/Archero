using Template.Features.Config.Remote;

namespace Template.Features.Backend.Socket
{
    public class SocketApiService : ISocketApiService
    {
        private readonly IRemoteConfigService _remoteConfigService;

        public SocketApiService(IRemoteConfigService remoteConfigService)
        {
            _remoteConfigService = remoteConfigService;
        }

        public string GetBaseUrl()
        {
            return _remoteConfigService.GetConfig<BackendRemoteFeatureConfig>(BackendRemoteFeatureConfig.Key).SocketUrl;
        }
    }
}