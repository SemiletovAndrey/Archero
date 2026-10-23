using System;
using Template.Features.ScreenNavigator;

namespace Menu.Features.MainMenu.Scripts
{
    public class MainMenuPayload : BaseNavigationItemPayload
    {
        public Action OnPlay { get; }
        
        public MainMenuPayload(Action onPlay)
        {
            OnPlay = onPlay;
        }
    }
}