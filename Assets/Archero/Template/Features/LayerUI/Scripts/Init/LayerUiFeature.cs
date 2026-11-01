using Template.Features.ProjectInit;

namespace Template.Features.LayerUI
{
    public class LayerUiFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IUILayersService, UILayersService>();
        }
    }
}