using Cysharp.Threading.Tasks;
using Template.Features.ScreenNavigator;
using UnityEngine;

namespace Menu.Features.MainMenu.Scripts
{
    public class MainMenuScreen : ActivatableScreen<MainMenuPayload>
    {
        public override UniTask Activate(MainMenuPayload data)
        {
            Debug.Log("Activating Main Menu");
            return UniTask.CompletedTask;
        }
    }
}