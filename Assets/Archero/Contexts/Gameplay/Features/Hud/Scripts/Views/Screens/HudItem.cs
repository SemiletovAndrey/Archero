using Template.Features.ScreenNavigator;

namespace Gameplay.Features.Hud
{
    [ScreenType(ScreenType.Hud)]
    public class HudItem : PresentedScreenNavigationItem<HudScreenPayload, IHudView, HudPresenter>
    {
        
    }
}