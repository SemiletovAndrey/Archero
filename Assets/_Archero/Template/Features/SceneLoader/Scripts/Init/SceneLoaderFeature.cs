using Template.Features.ProjectInit;

namespace Template.Features.SceneLoader
{
    public class SceneLoaderFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<ISceneLoader, SceneLoader>();
        }
    }
}