using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.Popups
{
    public interface IPopupSystem
    {
        public event Action<BasePopup> PopupOpened;
        
        public event Action<BasePopup> PopupClosed;
        
        public event Action AllClosed;
        
        public int OpenedCount { get; }

        public bool IsOpen(PopupType popupType);
        
        public void Init();
        
        public UniTask Open(PopupType popupType);
        public UniTask<TPopup> Open<TPopup>() where TPopup : BasePopupT<EmptyPopupPayload>;
        public UniTask<TPopup> Open<TPopup, TPayload>(TPayload payload)
            where TPopup : BasePopupT<TPayload>
            where TPayload : IPopupPayload;
    
        public UniTask Close(PopupType popupType);
        public UniTask Close<TPopup>() where TPopup : BasePopup;
        
        public UniTask CloseAll(params PopupType[] except);
        
        public UniTask Preload(PopupType popupType);
        public void Unload(PopupType popupType);
        public void UnloadAll(params PopupType[] except);
    }
}