using Template.Features.Config.Local;
using Template.Features.Container;
using Template.Features.DataStorage;
using Template.Features.Firebase.Init;
using Template.Features.LayerUI;
using Template.Features.Localization;
using Template.Features.NewInputFeature;
using Template.Features.Popups;
using Template.Features.ProjectInit;
using Template.Features.SceneLoader;

namespace Template.Scripts.Init
{
    public class TemplateFeatureGroup : BaseFeatureGroup
    {
        protected override void AddFeatures()
        {
            AddFeature<NewInputFeature>();
            
            AddFeature<SceneLoaderFeature>();
            
            AddFeature<LocalConfigFeature>();
            AddFeature<DataStorageFeature>();
            
            AddFeature<LocalizationFeature>();
            
            AddFeature<ContainerFeature>();
            AddFeature<PopupsFeature>();
            
            AddFeature<LayerUiFeature>();
            
            AddFeature<FirebaseFeature>();
        }
    }
}