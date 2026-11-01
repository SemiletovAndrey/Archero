using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.AppTime
{
    public class AppTimeFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IAppTimeService, AppTimeService>();
        }

        protected override UniTask InitServices()
        {
            AppTimeService appTimeService = ResolveAs<IAppTimeService, AppTimeService>();
            appTimeService.Init();
            
            AddProvider<LocalTimeProvider>(appTimeService);
            NtpTimeProvider ntpProvider = AddProvider<NtpTimeProvider>(appTimeService);
            ntpProvider.Init();
            
            return UniTask.CompletedTask;
        }

        private TProvider AddProvider<TProvider>(AppTimeService appTimeService)
            where TProvider : ITimeProvider
        {
            TProvider provider = CreateInstance<TProvider>();
            appTimeService.AddProvider(provider);

            return provider;
        }
    }
}