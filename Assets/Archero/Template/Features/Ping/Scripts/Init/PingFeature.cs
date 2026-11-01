using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.Ping
{
    public class PingFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IPingService, PingService>();
        }

        protected override async UniTask InitServices()
        {
            PingService service = ResolveAs<IPingService, PingService>();
            WebRequestPingProvider webRequestPingProvider = Container.Instantiate<WebRequestPingProvider>();
            
            await service.InitAsync(webRequestPingProvider);
        }
    }
}