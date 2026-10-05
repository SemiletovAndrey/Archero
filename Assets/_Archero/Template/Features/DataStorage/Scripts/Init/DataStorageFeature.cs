using Cysharp.Threading.Tasks;
using Template.Features.ProjectInit;

namespace Template.Features.DataStorage
{
    public class DataStorageFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IDataStorageService, DataStorageService>();
        }
        
        protected override UniTask InitServices()
        { 
            ResolveAs<IDataStorageService, DataStorageService>().Init();
            return UniTask.CompletedTask;
        }
    }
}