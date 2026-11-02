using Cysharp.Threading.Tasks;
using Template.Features.ScreenNavigator;
using Template.Features.UIExtensions;
using Template.Features.UILoader;
using UnityEngine;
using Zenject;

namespace Gameplay.Features.Hud
{
    public class HudScreenView : AnimatedActivatableScreen<HudScreenPayload>, IHudView
    {
        private ILoadingSystem _loadingSystem;
        private IScreenNavigator _screenNavigator;
        
        [SerializeField] private UIButton _button;

        [Inject]
        private void Construct(ILoadingSystem loadingSystem, IScreenNavigator screenNavigator)
        {
            _loadingSystem = loadingSystem;
            _screenNavigator = screenNavigator;
        }

        public override UniTask Activate(HudScreenPayload data)
        {
            _button.AssignAction(OpenTestScren);
            return UniTask.CompletedTask;
        }

        public override UniTask Open(NavigationItemOpenPayload payload)
        {
            _loadingSystem.Hide().Forget();
            return base.Open(payload);
        }

        private void OpenTestScren()
        {
            _screenNavigator.NavigateTo<HudInventoryItem, HudScreenPayload>(new()).Forget();
        }
    }
}