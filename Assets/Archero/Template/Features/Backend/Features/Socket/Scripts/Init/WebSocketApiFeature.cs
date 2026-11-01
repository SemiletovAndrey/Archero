using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.Backend.Socket
{
    public class WebSocketApiFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<ISocketApiService, SocketApiService>();
        }

        protected override UniTask InitServices()
        {
            ResolveAs<ISocketApiService, SocketApiService>();
            return UniTask.CompletedTask;
        }
    }
}