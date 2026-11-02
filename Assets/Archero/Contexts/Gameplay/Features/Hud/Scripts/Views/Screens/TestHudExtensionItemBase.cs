using Template.Features.ScreenNavigator;

namespace Gameplay.Features.Hud
{
    [ScreenType(ScreenType.Hud2)]
    public abstract class TestHudExtensionItemBase : ScreenNavigationItem<HudScreenPayload>
    {
        protected override ScreenType ResolveScreenType() => ScreenType.Hud2;
    }
}