using Gameplay.Features.Hud;
using Template.Features.ScreenNavigator;

namespace Menu.Features.MainMenu.Scripts
{
    [ScreenType(ScreenType.Menu)]
    public class MainMenuItem : PresentedScreenNavigationItem<MainMenuScreenPayload, IMainMenuView, MainMenuPresenter>
    {
        protected override NavigationItemList AddInnerItems()
        {
            return new NavigationItemList()
                .Add<MenuInventoryItem>();
        }
    }
}