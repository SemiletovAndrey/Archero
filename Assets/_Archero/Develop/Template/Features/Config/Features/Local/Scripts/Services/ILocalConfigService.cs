namespace Template.Features.Config.Local
{
    public interface ILocalConfigService
    {
        public TConfig GetConfig<TConfig>() where TConfig : ILocalFeatureConfig;
        
        public void AddProvider(ILocalConfigProvider provider);
        public void RemoveProvider(ILocalConfigProvider provider);
    }
}