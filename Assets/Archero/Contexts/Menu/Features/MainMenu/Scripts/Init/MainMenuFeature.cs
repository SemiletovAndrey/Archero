using Template.Features.ProjectInit;

namespace Menu.Features.MainMenu
{
    public class MainMenuFeature : BaseFeature
    {
        protected override void InstallBindings()
        {
            BindAsSingle<IMainMenuSystem, MainMenuSystem>();
        }
    }
}