using System;
using Persistance.Features.Wallet;

namespace Menu.Features.MainMenu.Scripts
{
    public interface IMainMenuView
    {
        public WalletView WalletView { get; }
        
        public event Action PlayClicked;
        public event Action OpenPopupClicked;
    }
}