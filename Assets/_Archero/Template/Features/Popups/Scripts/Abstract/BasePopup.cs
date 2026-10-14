using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.Popups
{
    public abstract class BasePopup : MonoBehaviour
    {
        private PopupType _popupType;

        internal virtual Type PayloadType => typeof(EmptyPopupPayload);
        
        private Action<PopupType> _closeExternalHandler;
        
        public void SetPopupType(PopupType popupType)
        {
            _popupType = popupType;
        }
        
        public abstract UniTask Deactivate();

        public abstract UniTask Show();

        public abstract UniTask Hide();

        internal void SetCloseHandler(Action<PopupType> closeHandler)
        {
            _closeExternalHandler = closeHandler;
        }

        internal virtual void NotifyOpening()
        {
        }

        internal virtual void NotifyClosed()
        {
        }

        public void CloseExternal()
        {
            if (_closeExternalHandler != null)
            {
                _closeExternalHandler.Invoke(_popupType);
            }
            else
            {
                Debug.LogWarning($"[{nameof(BasePopup)}] CloseExternal вызван для '{_popupType}', " +
                                 "но обработчик закрытия не назначен. Похоже, попап был открыт не через IPopupSystem.");
            }
        }
    }
}