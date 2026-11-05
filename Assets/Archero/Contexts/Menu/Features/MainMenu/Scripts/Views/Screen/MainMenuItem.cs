using Persistance.Features.LevelProgression;
using Template.Features.ScreenNavigator;

namespace Menu.Features.MainMenu.Scripts
{
    [ScreenType(ScreenType.MenuScreen)]
    public class MainMenuItem : PresentedScreenNavigationItem<MainMenuScreenPayload, IMainMenuView, MainMenuScreenPresenter>
    {
        protected override NavigationItemList AddInnerItems()
        {
            return new NavigationItemList()
                .Add<ChoiceLevelItem>();
        }
    }
}