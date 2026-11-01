using Persistance.Features.Auth;
using Template.Features.ScreenNavigator;

namespace Persistence.Features.Auth
{
    [ScreenType(ScreenType.SignInScreen)]
    public class SignInScreenItem : PresentedScreenNavigationItem<SignInScreenPayload, ISignInView, SignInPresenter>
    {
    }
}