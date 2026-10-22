using Cysharp.Threading.Tasks;
using Template.Features.Ping;
using Template.Features.ProjectInit;

namespace Template.Features.Backend.Ping
{
    public class BackendPingFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            
        }
        
        protected override UniTask InitServices()
        {
            AddRemoteConfigProvider();
            return UniTask.CompletedTask;
        }

        private void AddRemoteConfigProvider()
        {
            HttpPingProvider httpPingProvider = Container.Instantiate<HttpPingProvider>();
            Container.Resolve<IPingService>().AddProvider(httpPingProvider);
        }
    }
}