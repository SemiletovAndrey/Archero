using Persistence.Features.Auth;
using Template.Features.ScreenNavigator;

namespace Auth.Scripts.Navigation
{
    public class AuthNavigationRoot : NavigationRoot
    {
        protected override NavigationItemList AddInnerItems()
        {
            return new NavigationItemList()
                .Add<SignInScreenItem>();
        }

    }
}