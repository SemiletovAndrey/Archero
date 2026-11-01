using System;
using Cysharp.Threading.Tasks;
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
        
        private ILoadingSystem _loadingSystem;

        [Inject]
        private void Construct(ILoadingSystem loadingSystem)
        {
            _loadingSystem = loadingSystem;
        }
        
        public event Action PlayClicked;
        
        public override UniTask Activate(MainMenuScreenPayload data)
        {
            _playButton.AssignAction(OnPlayClicked);
            return UniTask.CompletedTask;
        }

        public override UniTask Open(NavigationItemOpenPayload payload)
        {
            _loadingSystem.Hide().Forget();
            return base.Open(payload);
        }

        private void OnPlayClicked()
        {
            PlayClicked?.Invoke();
        }

    }
}