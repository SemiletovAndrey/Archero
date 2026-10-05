namespace Template.Features.Config.Remote
{
    public interface IRemoteConfigProvider
    {
        public int Priority { get; }

        public bool TryGetConfig<TConfig>(string key, out TConfig config) where TConfig : class, IRemoteFeatureConfig;
    }
}