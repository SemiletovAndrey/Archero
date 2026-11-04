using Template.Features.ScreenNavigator;

namespace Gameplay.Features.Hud
{
    [ScreenType(ScreenType.HudScreen)]
    public class HudItem : PresentedScreenNavigationItem<HudScreenPayload, IHudView, HudScreenPresenter>
    {

    }
}