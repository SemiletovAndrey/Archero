using Cysharp.Threading.Tasks;
using Template.Features.ScreenNavigator;
using Template.Features.UIExtensions;
using UnityEngine;
using Zenject;

namespace Gameplay.Features.Hud
{
    public class TestHudExtensionView : AnimatedActivatableScreen<HudScreenPayload>, IHud2View
    {
        private IScreenNavigator _navigator;
        
        [SerializeField] private UIButton _button;

        [Inject]
        private void Construct(IScreenNavigator navigator)
        {
            _navigator = navigator;
        }
        
        public override UniTask Activate(HudScreenPayload data)
        {
            return UniTask.CompletedTask;
        }

        public override UniTask Open(NavigationItemOpenPayload payload)
        {
            _button.AssignAction(BackToTheHudMain);
            return base.Open(payload);
        }

        public override UniTask Close(NavigationItemClosePayload payload)
        {
            return base.Close(payload);
        }

        private void BackToTheHudMain()
        {
            _navigator.NavigateTo<HudItem, HudScreenPayload>(new()).Forget();
        }
    }
}