using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;

namespace Template.Features.Popups
{
    public class InstantiatePopupFactory : IPopupFactory
    {
        private Transform _parent;
        
        private readonly IClassFactory  _classFactory;
        private readonly IUILayersService _layersService;

        public InstantiatePopupFactory(IClassFactory classFactory, IUILayersService layersService)
        {
            _classFactory = classFactory;
            _layersService = layersService;
        }

        public void Init(Transform parent)
        {
            _parent = parent;
        }

        public UniTask<T> Create<T>(GameObject prefab) where T : BasePopup
        {
            GameObject instance = Object.Instantiate(prefab, _parent);
            Canvas canvas = instance.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = _layersService.GetNextSortingOrder(UILayerType.Popup);
            }
            return UniTask.FromResult(instance.GetComponent<T>());
        }

        public void Destroy(BasePopup popup)
        {
            Object.Destroy(popup.gameObject);
        }
    }
}