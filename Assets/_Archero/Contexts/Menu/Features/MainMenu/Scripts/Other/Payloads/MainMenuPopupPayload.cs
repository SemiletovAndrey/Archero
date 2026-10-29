using Template.Features.Popups;

namespace Menu.Features.MainMenu.Scripts
{
    public class MainMenuPopupPayload : IPopupPayload
    {
        public IMainMenuSystem MainMenu { get; private set; }

        public MainMenuPopupPayload(IMainMenuSystem mainMenu)
        {
            MainMenu = mainMenu;
        }
    }
}