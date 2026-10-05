using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.Cheat
{
    public class CheatFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<ICheatService, CheatService>();
        }

        protected override void AddCheats()
        {
            CreateAndInitCheat<SampleCheatHandler>();
        }

        protected override UniTask InitServices()
        {
            ResolveAs<ICheatService, CheatService>().Init();
            return UniTask.CompletedTask;
        }
    }
}
