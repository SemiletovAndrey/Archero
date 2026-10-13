using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.Popups
{
    public abstract class BasePopup : MonoBehaviour
    {
        [field: SerializeField] public PopupType PopupType { get; set; }

        internal virtual Type PayloadType => typeof(EmptyPopupPayload);
        
        private Action<PopupType> _closeExternalHandler;
        
        public abstract UniTask Deactivate();

        public abstract UniTask Show();

        public abstract UniTask Hide();
        
        internal void SetCloseHandler(Action<PopupType> closeHandler)
        {
            _closeExternalHandler = closeHandler;
        }
        
        public void CloseExternal()
        {
            if (_closeExternalHandler != null)
            {
                _closeExternalHandler.Invoke(PopupType);
            }
            else
            {
                Debug.LogWarning($"[{nameof(BasePopup)}] CloseExternal вызван для '{PopupType}', " +
                                 "но обработчик закрытия не назначен. Похоже, попап был открыт не через IPopupSystem.");
            }
        }

    }
}