using Cysharp.Threading.Tasks;
using Template.Features.Popups;
using UnityEngine;

namespace Contexts.Persistance.Auth
{
    [PopupType(PopupType.Test)]
    public class TestPopup : BasePopup<TestPopupPayload>
    {
        public override UniTask Deactivate()
        {
            return UniTask.CompletedTask;
        }

        public override UniTask Show()
        {
            
            return UniTask.CompletedTask;
        }

        public override UniTask Hide()
        {
            return UniTask.CompletedTask;
        }

        public override UniTask Activate(TestPopupPayload payload)
        {
            Debug.Log($"TestPopup {payload.Value}");
            return UniTask.CompletedTask;
        }
    }
}