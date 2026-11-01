namespace Template.Features.Config.Remote
{
    public interface IRemoteConfigService
    {
        public T GetConfig<T>(string key) where T : class, IRemoteFeatureConfig;
        public bool TryGetConfig<TConfig>(string key, out TConfig config) where TConfig : class, IRemoteFeatureConfig;

        public void AddProvider(IRemoteConfigProvider provider);
        public void RemoteProvider(IRemoteConfigProvider provider);
        
        public TProvider GetProvider<TProvider>() where TProvider : IRemoteConfigProvider;
    }
}