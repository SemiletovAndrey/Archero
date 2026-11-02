using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Persistance.Features.LevelProgression
{
    public class LevelProgressionFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<ILevelProgressionService, LevelProgressionService>();
        }

        protected override UniTask InitServices()
        {
            ResolveAs<ILevelProgressionService, LevelProgressionService>().Init();
            return base.InitServices();
        }
    }
}