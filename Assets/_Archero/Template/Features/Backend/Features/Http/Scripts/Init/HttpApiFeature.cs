using Cysharp.Threading.Tasks;
using Persistence.Features.Auth.Scripts;
using Template.Features.ProjectInit;

namespace Template.Features.Backend.Http
{
    public class HttpApiFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IHttpApiService, HttpApiService>();
        }

        protected override UniTask InitServices()
        {
            HttpApiService service = ResolveAs<IHttpApiService, HttpApiService>();
            service.Init();
            
            AddUrlProviders(service);
            AddTokenProviders(service);
            
            return UniTask.CompletedTask;
        }

        private void AddUrlProviders(IHttpApiService service)
        {
            RemoteConfigHttpUrlProvider remoteConfigUrlProvider = CreateInstance<RemoteConfigHttpUrlProvider>();
            service.AddUrlProvider(remoteConfigUrlProvider);
        }
            
        private void AddTokenProviders(IHttpApiService service)
        {
            FirebaseServerTokenProvider emptyTokenProvider = CreateInstance<FirebaseServerTokenProvider>();
            service.AddTokenProvider(emptyTokenProvider);
        }
    }
}