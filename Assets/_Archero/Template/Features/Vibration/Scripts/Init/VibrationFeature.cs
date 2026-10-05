using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.Vibration
{
    public class VibrationFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IVibrationService, VibrationService>();
            BindAsSingle<IVibrationSystem, VibrationSystem>();
        }

        protected override UniTask InitServices()
        {
            ResolveAs<IVibrationService, VibrationService>().Init();
            return UniTask.CompletedTask;
        }

        protected override UniTask InitSystems()
        {
            IVibrationHandler handler = CreateInstance<EmptyVibrationHandler>();
            ResolveAs<IVibrationSystem, VibrationSystem>().Init(handler);
            
            return UniTask.CompletedTask;
        }
    }
}