using System;

namespace Menu.Features.MainMenu.Scripts
{
    public interface IMainMenuView
    {
        public event Action PlayClicked;
        event Action OpenPopupClicked;
    }
}