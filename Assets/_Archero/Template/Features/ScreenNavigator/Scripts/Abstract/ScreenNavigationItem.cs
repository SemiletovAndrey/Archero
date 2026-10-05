namespace Template.Features.ScreenNavigator
{
    public abstract class ScreenNavigationItem<TPayload> : BaseNavigationItem<TPayload>
        where TPayload : BaseNavigationItemPayload, new()
    {

    }
}