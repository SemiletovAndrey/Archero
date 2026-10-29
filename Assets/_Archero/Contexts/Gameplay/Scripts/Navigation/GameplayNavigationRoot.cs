using Gameplay.Features.Hud;
using Template.Features.ScreenNavigator;

namespace Gameplay.Scripts
{
    public class GameplayNavigationRoot : NavigationRoot
    {
        protected override NavigationItemList AddInnerItems()
        {
            return new NavigationItemList()
                .Add<HudItem>();
        }
    }
}