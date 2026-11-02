using Cysharp.Threading.Tasks;
using Template.Features.Presenters;
using UnityEngine;

namespace Gameplay.Features.Hud
{
    public class HudPresenter : IPresenter<HudScreenPayload>
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