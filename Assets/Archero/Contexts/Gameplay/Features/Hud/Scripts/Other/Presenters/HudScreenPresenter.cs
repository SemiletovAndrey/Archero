using Cysharp.Threading.Tasks;
using Template.Features.Presenters;
using UnityEngine;

namespace Gameplay.Features.Hud
{
    public class HudScreenPresenter : IScreenPresenter<HudScreenPayload>
    {
        public UniTask Activate(HudScreenPayload payload)
        {
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            
        }
    }
}