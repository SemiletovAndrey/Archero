using System;
using Cysharp.Threading.Tasks;
using Template.Features.ScreenNavigator;
using Template.Features.UIExtensions;
using UnityEngine;

namespace Menu.Features.MainMenu.Scripts
{
    public class MainMenuScreenView : ActivatableScreen<MainMenuScreenPayload>, IMainMenuView
    {
        [SerializeField] private UIButton _playButton;
        
        public event Action PlayClicked;
        
        public override UniTask Activate(MainMenuScreenPayload data)
        {
            _playButton.AssignAction(OnPlayClicked);
            return UniTask.CompletedTask;
        }

        private void OnPlayClicked()
        {
            PlayClicked?.Invoke();
        }

    }
}