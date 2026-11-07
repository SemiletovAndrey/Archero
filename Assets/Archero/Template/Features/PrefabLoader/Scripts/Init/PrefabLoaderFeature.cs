using Template.Features.ProjectInit;

namespace Template.Features.PrefabLoader
{
    public class PrefabLoaderFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IPrefabLoader, PrefabProvider>();
        }
    }
}