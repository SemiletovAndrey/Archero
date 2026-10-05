using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.ClassFactory
{
    public class ClassFactoryFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IClassFactory, ClassFactory>();
        }

        protected override UniTask InitServices()
        {
            ResolveAs<IClassFactory, ClassFactory>().Init();
            return UniTask.CompletedTask;
        }
    }
}