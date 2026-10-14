using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.Popups
{
    public interface IPopupFactory
    {
        public void Init(Transform parent);
        
        public UniTask<T> Create<T>(PopupEntryBase entry) where T : BasePopup;
        
        public void Release(BasePopup popup);
        
        public UniTask Preload(PopupEntryBase entry);
        
        public void Unload(PopupEntryBase entry);
    }
}