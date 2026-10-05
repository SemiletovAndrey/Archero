using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;
using UnityEngine;

namespace Template.Features.Config.Remote
{
    public class RemoteConfigFeature : BaseMonoBehaviourFeature
    {
        [SerializeField] private BaseRemoteConfigProvider _defaultProvider;
        [SerializeField] private BaseRemoteConfigProvider _overrideProvider;

        protected override void InstallBindings()
        {
            BindAsSingle<IRemoteConfigService, RemoteConfigService>();
        }

        protected override UniTask InitServices()
        {
            RemoteConfigService service = ResolveAs<IRemoteConfigService, RemoteConfigService>();
            service.Init();
            
            AddDefaultProvider(service);
            AddOverrideProvider(service);

            return UniTask.CompletedTask;
        }

        private void AddDefaultProvider(RemoteConfigService service)
        {
            AddProvider(_defaultProvider, service);
        }

        private void AddOverrideProvider(RemoteConfigService service)
        {
            AddProvider(_overrideProvider, service);
        }

        private void AddProvider(BaseRemoteConfigProvider provider, RemoteConfigService service)
        {
            if (provider == null)
            {
                return;
            }
            
            provider.Init();
            service.AddProvider(provider);
        }
    }
}