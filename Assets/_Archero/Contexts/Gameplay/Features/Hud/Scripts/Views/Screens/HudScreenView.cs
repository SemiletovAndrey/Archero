using Cysharp.Threading.Tasks;
using Template.Features.ScreenNavigator;

namespace Gameplay.Features.Hud
{
    public class HudScreenView : ActivatableScreen<HudScreenPayload>, IHudView
    {
        public override UniTask Activate(HudScreenPayload data)
        {
            return UniTask.CompletedTask;
        }
    }
}