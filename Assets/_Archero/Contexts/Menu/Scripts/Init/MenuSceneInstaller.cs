using Template.Features.GameFlow.Scripts;
using Template.Features.ProjectInit;

namespace _Archero.Contexts.Menu.Scripts.Init
{
    public class MenuSceneInstaller : BaseInstaller
    {
        protected override void AddFeatures()
        {
            Container.BindInstance(new SceneFeatureSet(GetFeatures())).AsSingle();
        }
    }
}