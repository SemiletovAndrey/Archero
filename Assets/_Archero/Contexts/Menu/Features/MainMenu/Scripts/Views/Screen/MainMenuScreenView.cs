using Cysharp.Threading.Tasks;
using Template.Features.ScreenNavigator;

namespace Menu.Features.MainMenu.Scripts
{
    public class MainMenuScreenView : ActivatableScreen<MainMenuPayload>, IMainMenuView
    {
        public override UniTask Activate(MainMenuPayload data)
        {
            return UniTask.CompletedTask;
        }
    }
}