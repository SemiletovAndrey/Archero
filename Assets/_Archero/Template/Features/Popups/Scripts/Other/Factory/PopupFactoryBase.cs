using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;

namespace Template.Features.Popups
{
    public abstract class PopupFactoryBase : IPopupFactory
    {
        private Transform _parent;

        private readonly IClassFactory _classFactory;
        private readonly IUILayersService _layersService;

        protected PopupFactoryBase(IClassFactory classFactory, IUILayersService layersService)
        {
            _classFactory = classFactory;
            _layersService = layersService;
        }

        public void Init(Transform parent)
        {
            _parent = parent;
        }

        public abstract bool Supports(PopupEntryBase entry);

        public abstract UniTask<T> Create<T>(PopupEntryBase entry) where T : BasePopup;

        public abstract void Destroy(BasePopup popup);

        public virtual UniTask Preload(PopupEntryBase entry)
        {
            return UniTask.CompletedTask;
        }

        public virtual void Unload(PopupEntryBase entry)
        {
        }

        protected void EnsureReady(PopupEntryBase entry)
        {
            if (_parent == null)
                throw new System.InvalidOperationException(
                    $"{GetType().Name} is not initialized. Call Init(parent) first.");

            EnsureSource(entry);
        }

        protected static void EnsureSource(PopupEntryBase entry)
        {
            if (entry == null)
                throw new System.ArgumentNullException(nameof(entry));

            if (!entry.HasSource)
                throw new System.InvalidOperationException($"PopupType.{entry.Type}: prefab source is not assigned.");
        }

        protected T InstantiatePopup<T>(GameObject prefab, PopupEntryBase entry) where T : BasePopup
        {
            BasePopup instance = _classFactory.CreateMonoBehaviour<BasePopup>(prefab, _parent);
            if (instance == null)
                throw new System.InvalidOperationException(
                    $"PopupType.{entry.Type}: failed to create instance from prefab '{prefab.name}'.");

            if (!instance.TryGetComponent(out T popup))
            {
                Object.Destroy(instance.gameObject);
                throw new System.InvalidOperationException(
                    $"Prefab '{prefab.name}' (PopupType.{entry.Type}) has no component {typeof(T).Name}.");
            }

            if (instance.TryGetComponent(out Canvas canvas))
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = _layersService.GetNextSortingOrder(UILayerType.Popup);
            }
            else
            {
                Debug.LogWarning($"[{GetType().Name}] Prefab '{prefab.name}' (PopupType.{entry.Type}) " +
                                 "has no Canvas on the root object - sorting order was not applied.");
            }

            return popup;
        }

    }
}