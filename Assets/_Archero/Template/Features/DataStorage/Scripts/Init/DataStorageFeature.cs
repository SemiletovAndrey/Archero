using Template.Features.ProjectInit;

namespace Template.Features.DataStorage
{
    public class DataStorageFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IDataKeyStorage, MapDataKeyStorage>();
            BindAsSingle<IDataSerializer, JsonSerializer>();
            BindAsSingle<IDataRepository, LocalFileDataRepository>().WithArguments(DataStorageConstants.FolderPath,
                DataStorageConstants.SaveFileExtension);
            BindAsSingle<ISaveLoadService, SaveLoadService>();
        }
    }
}