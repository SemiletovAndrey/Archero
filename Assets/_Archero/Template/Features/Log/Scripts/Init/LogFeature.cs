using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.Log
{
    public class LogFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<ILogService, LogService>();
            BindAsCashed<IDebugLogger, SimpleLogger>();
        }

        protected override UniTask InitServices()
        {
            ResolveAs<ILogService, LogService>().Init();
            return UniTask.CompletedTask;
        }
    }
}