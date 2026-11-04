using System;
using Cysharp.Threading.Tasks;
using Persistance.Features.Wallet;
using Template.Features.ScreenNavigator;
using Template.Features.UIExtensions;
using Template.Features.UILoader;
using UnityEngine;
using Zenject;

namespace Menu.Features.MainMenu.Scripts
{
    public class MainMenuScreenView : AnimatedActivatableScreen<MainMenuScreenPayload>, IMainMenuView
    {
        [SerializeField] private UIButton _playButton;
        [SerializeField] private UIButton _openPopupButton;
        
        [SerializeField] private WalletView _walletView;
        
        private ILoadingSystem _loadingSystem;

        [Inject]
        private void Construct(ILoadingSystem loadingSystem)
        {
            _loadingSystem = loadingSystem;
        }
        
        public event Action PlayClicked;
        public event Action OpenPopupClicked;
        
        public WalletView WalletView => _walletView;
        
        public override UniTask Activate(MainMenuScreenPayload data)
        {
            _playButton.AssignAction(OnPlayClicked);
            _openPopupButton.AssignAction(OnOpenPopupClicked);
            return UniTask.CompletedTask;
        }

        public override UniTask Open(NavigationItemOpenPayload payload)
        {
            _loadingSystem.Hide().Forget();
            return base.Open(payload);
        }

        public override UniTask Close(NavigationItemClosePayload payload)
        {
            _playButton.SetAvailability(false);
            return base.Close(payload);
        }

        private void OnPlayClicked()
        {
            PlayClicked?.Invoke();
        }

        private void OnOpenPopupClicked()
        {
            OpenPopupClicked?.Invoke();
        }
    }
}