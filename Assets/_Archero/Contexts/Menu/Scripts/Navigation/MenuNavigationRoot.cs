using Menu.Features.MainMenu.Scripts;
using Template.Features.ScreenNavigator;

namespace Menu.Scripts.Navigation
{
    public class MenuNavigationRoot : NavigationRoot
    {
        protected override NavigationItemList AddInnerItems()
        {
            return new NavigationItemList()
            .Add<MainMenuItem>();
        }

    }
}