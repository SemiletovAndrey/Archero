using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.Localization
{
    public class LocalizationFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<ILocalizationService, LocalizationService>();
        }
        
        protected override async UniTask InitServices()
        {
            await ResolveAs<ILocalizationService, LocalizationService>().InitAsync();
        }
    }
}