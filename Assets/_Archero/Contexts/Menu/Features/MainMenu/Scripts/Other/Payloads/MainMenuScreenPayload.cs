using System;
using Template.Features.ScreenNavigator;

namespace Menu.Features.MainMenu.Scripts
{
    public class MainMenuScreenPayload : BaseNavigationItemPayload
    {
        public Action OnPlay { get; }
        
        public MainMenuScreenPayload(Action onPlay)
        {
            OnPlay = onPlay;
        }
    }
}