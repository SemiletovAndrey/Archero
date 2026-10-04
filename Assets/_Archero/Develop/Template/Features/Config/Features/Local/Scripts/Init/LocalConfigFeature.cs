using UnityEngine;

namespace Template.Features.Config.Local
{
    public class LocalConfigFeature //: BaseMonoBehaviourFeature
    {
        /*[SerializeField] private ContextLocalConfigProvider _projectContextConfigProvider;

        protected override void InstallBindings()
        {
            BindAsSingle<ILocalConfigService, LocalConfigService>();
        }

        protected override UniTask InitServices()
        {
            LocalConfigService configService = ResolveAs<ILocalConfigService, LocalConfigService>();
            
            configService.Init();
            configService.AddProvider(_projectContextConfigProvider);
            
            return UniTask.CompletedTask;
        }*/
    }
}