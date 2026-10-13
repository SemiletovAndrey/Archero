using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.Popups
{
    public interface IPopupFactory
    {
        public void Init(Transform parent);

        public bool Supports(PopupEntryBase entry);

        public UniTask<T> Create<T>(PopupEntryBase entry) where T : BasePopup;

        public void Destroy(BasePopup popup);

        
        public UniTask Preload(PopupEntryBase entry);
        
        public void Unload(PopupEntryBase entry);

    }
}