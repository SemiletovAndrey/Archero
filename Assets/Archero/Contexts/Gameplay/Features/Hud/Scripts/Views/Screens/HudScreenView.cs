using Cysharp.Threading.Tasks;
using Menu.Features.MainMenu.Scripts;
using Menu.Scripts;
using Template.Features.ScreenNavigator;
using Template.Features.UIExtensions;
using Template.Features.UILoader;
using UnityEngine;
using Zenject;

namespace Gameplay.Features.Hud
{
    public class HudScreenView : ActivatableScreen<HudScreenPayload>, IHudView
    {
        private ILoadingSystem _loadingSystem;
        
        [SerializeField] private UIButton _button;

        [Inject]
        private void Construct(ILoadingSystem loadingSystem)
        {
            _loadingSystem = loadingSystem;
        }

        public override UniTask Activate(HudScreenPayload data)
        {
            _button.AssignAction(BackMenu);
            return UniTask.CompletedTask;
        }

        public override UniTask Open(NavigationItemOpenPayload payload)
        {
            _loadingSystem.Hide().Forget();
            return base.Open(payload);
        }

        private void BackMenu()
        {
            
        }
    }
}