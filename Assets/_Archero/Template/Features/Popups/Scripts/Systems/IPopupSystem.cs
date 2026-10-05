using Cysharp.Threading.Tasks;

namespace Template.Features.Popups
{
    public interface IPopupSystem
    {
        public void Init();
        
        public UniTask Open(PopupType popupType);
        public UniTask<TPopup> Open<TPopup>() where TPopup : BasePopup<EmptyPopupPayload>;
        public UniTask<TPopup> Open<TPopup, TPayload>(TPayload payload)
            where TPopup : BasePopup<TPayload>
            where TPayload : IPopupPayload;
    
        public UniTask Close(PopupType popupType);
        public UniTask Close<TPopup>() where TPopup : BasePopup;
    }
}