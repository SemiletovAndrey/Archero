using Template.Features.ScreenNavigator;

namespace Gameplay.Features.Hud
{
    [ScreenType(ScreenType.HudScreen)]
    public class HudItem : PresentedScreenNavigationItem<HudScreenPayload, IHudView, HudPresenter>
    {
        protected override NavigationItemList AddInnerItems()
        {
            return new NavigationItemList()
                .Add<HudInventoryItem>();
        }
    }
}