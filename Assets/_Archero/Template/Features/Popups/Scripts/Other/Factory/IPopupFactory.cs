using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.Popups
{
    public interface IPopupFactory
    {
        public void Init(Transform parent);
        
        public UniTask<T> Create<T>(GameObject prefab) where T : BasePopup;
        
        public void Destroy(BasePopup popup);
    }
}