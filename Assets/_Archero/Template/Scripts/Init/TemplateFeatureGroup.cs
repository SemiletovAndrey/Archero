using Template.Features.AppTime;
using Template.Features.ClassFactory;
using Template.Features.Config.Local;
using Template.Features.Config.Remote;
using Template.Features.Container;
using Template.Features.DataStorage;
using Template.Features.LayerUI;
using Template.Features.Localization;
using Template.Features.Ping;
using Template.Features.Popups;
using Template.Features.ProjectInit;
using Template.Features.SceneLoader;
using Template.Features.ScreenNavigator;
using Template.Features.Vibration;

namespace Template.Scripts.Init
{
    public class TemplateFeatureGroup : BaseFeatureGroup
    {
        protected override void AddFeatures()
        {
            AddFeature<SceneLoaderFeature>();
            
            AddFeature<LocalConfigFeature>();
            AddFeature<DataStorageFeature>();
            
            AddFeature<LocalizationFeature>();
            
            AddFeature<ContainerFeature>();
            AddFeature<PopupsFeature>();
            //AddFeature<ScreenNavigatorFeature>();
            
            AddFeature<LayerUiFeature>();
        }
    }
}