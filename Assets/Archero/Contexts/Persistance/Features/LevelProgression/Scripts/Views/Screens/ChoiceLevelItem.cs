using Template.Features.ScreenNavigator;

namespace Persistance.Features.LevelProgression
{
    [ScreenType(ScreenType.ChoiceLevelScreen)]
    public class ChoiceLevelItem : PresentedScreenNavigationItem<ChoiceLevelPayload, IChoiceLevelScreenView,
        ChoiceLevelPresenter>
    {
        
    }
}