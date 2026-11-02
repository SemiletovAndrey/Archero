using Gameplay.Features.Hud;
using Persistance.Features.LevelProgression;
using Template.Features.ScreenNavigator;

namespace Menu.Features.MainMenu.Scripts
{
    [ScreenType(ScreenType.MenuScreen)]
    public class MainMenuItem : PresentedScreenNavigationItem<MainMenuScreenPayload, IMainMenuView, MainMenuPresenter>
    {
        protected override NavigationItemList AddInnerItems()
        {
            return new NavigationItemList()
                .Add<MenuInventoryItem>()
                .Add<ChoiceLevelItem>();
        }
    }
}