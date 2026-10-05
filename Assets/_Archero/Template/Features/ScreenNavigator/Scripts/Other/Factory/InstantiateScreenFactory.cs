using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public class InstantiateScreenFactory : IScreenFactory
    {
        private readonly IClassFactory _classFactory;
        private readonly IUILayersService _uiLayersService;
        
        private Transform _defaultParent;
        
        private readonly Dictionary<ScreenType, Stack<BaseScreen>> _pool = new();

        public InstantiateScreenFactory(IClassFactory classFactory, IUILayersService uiLayersService)
        {
            _classFactory = classFactory;
            _uiLayersService = uiLayersService;
        }
        
        public void Init(Transform parent)
        {
            _defaultParent = parent;
        }

        public async UniTask<T> Create<T>(ScreenType screenType, GameObject prefab, Transform parent = null)
            where T : Component
        {
            if (_pool.TryGetValue(screenType, out var stack) && stack.Count > 0)
            {
                BaseScreen instance = stack.Pop();
                instance.gameObject.SetActive(true);
                ApplySortingOrder(instance.gameObject, UILayerType.Screen);
                return instance as T;
            }

            T component = _classFactory.CreateMonoBehaviour<T>(prefab, parent ?? _defaultParent);
            ApplySortingOrder(component.gameObject, UILayerType.Screen);

            if (component is BaseScreen baseScreen)
                baseScreen.Init(screenType);

            return component;
        }

        public void Destroy<T>(T instance, ScreenType screenType) where T : Component
        {
            if (instance == null) return;
            instance.gameObject.SetActive(false);

            var screen = instance as BaseScreen ?? instance.GetComponent<BaseScreen>();
            if (screen != null)
            {
                if (!_pool.ContainsKey(screenType))
                    _pool[screenType] = new Stack<BaseScreen>();
                _pool[screenType].Push(screen);
            }
        }

        private void ApplySortingOrder(GameObject go, UILayerType type)
        {
            Canvas canvas = go.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = _uiLayersService.GetNextSortingOrder(type);
            }
        }
    }
}