using Cysharp.Threading.Tasks;
using Template.Features.Backend.Http;
using Template.Features.Config.Remote;
using Template.Features.ProjectInit;

namespace Template.Features.Backend.RemoteConfig
{
    public class BackendRemoteConfigFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            
        }

        protected override UniTask InitServices()
        {
            return AddRemoteConfigProvider();
        }

        private async UniTask AddRemoteConfigProvider()
        {
            HttpRemoteConfigProvider httpConfigProvider = CreateInstance<HttpRemoteConfigProvider>();
            await httpConfigProvider.InitAsync();
            
            Container.Resolve<IRemoteConfigService>().AddProvider(httpConfigProvider);
        }
    }
}