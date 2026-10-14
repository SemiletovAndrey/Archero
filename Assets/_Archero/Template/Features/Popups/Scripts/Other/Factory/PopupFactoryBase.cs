using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;
using UnityEngine.UI;

namespace Template.Features.Popups
{
    public abstract class PopupFactoryBase : IPopupFactory
    {
        private const int NoSortingOrder = -1;

        private sealed class PopupInstance
        {
            public readonly PopupEntryBase Entry;
            public int SortingOrder = NoSortingOrder;

            public PopupInstance(PopupEntryBase entry)
            {
                Entry = entry;
            }
        }

        private Transform _parent;

        private readonly IClassFactory _classFactory;
        private readonly IUILayersService _layersService;
        
        private readonly Dictionary<BasePopup, PopupInstance> _instances = new();
        
        private readonly Dictionary<PopupType, BasePopup> _pool = new();

        protected PopupFactoryBase(IClassFactory classFactory, IUILayersService layersService)
        {
            _classFactory = classFactory;
            _layersService = layersService;
        }

        public void Init(Transform parent)
        {
            _parent = parent;
        }

        #region Source
        
        protected abstract UniTask<GameObject> AcquirePrefab(PopupEntryBase entry);
        
        protected abstract void ReleasePrefab(PopupEntryBase entry);
        
        public virtual UniTask Preload(PopupEntryBase entry)
        {
            return UniTask.CompletedTask;
        }

        public virtual void Unload(PopupEntryBase entry)
        {
            if (entry == null || !_pool.TryGetValue(entry.Type, out BasePopup pooled))
                return;

            _pool.Remove(entry.Type);

            if (_instances.TryGetValue(pooled, out PopupInstance info))
                DisposeInstance(pooled, info);
            else if (pooled != null)
                Object.Destroy(pooled.gameObject);
        }

        #endregion

        public async UniTask<T> Create<T>(PopupEntryBase entry) where T : BasePopup
        {
            EnsureReady(entry);

            if (entry.Pooled && TryTakeFromPool(entry, out T pooled))
                return pooled;

            GameObject prefab = await AcquirePrefab(entry);

            T popup = null;
            try
            {
                popup = InstantiatePopup<T>(prefab, entry);

                PopupInstance info = new PopupInstance(entry);
                _instances[popup] = info;
                info.SortingOrder = AssignSortingOrder(popup);

                return popup;
            }
            catch
            {
                if (popup != null)
                {
                    _instances.Remove(popup);
                    Object.Destroy(popup.gameObject);
                }

                ReleasePrefab(entry);
                throw;
            }
        }

        public void Release(BasePopup popup)
        {
            if (ReferenceEquals(popup, null))
                return;

            if (!_instances.TryGetValue(popup, out PopupInstance info))
            {
                if (popup != null)
                    Object.Destroy(popup.gameObject);

                return;
            }

            ReleaseSortingOrder(info);

            PopupEntryBase entry = info.Entry;
            if (entry.Pooled && popup != null && !_pool.ContainsKey(entry.Type))
            {
                popup.gameObject.SetActive(false);
                _pool[entry.Type] = popup;
                return;
            }

            DisposeInstance(popup, info);
        }

        #region Instances

        private T InstantiatePopup<T>(GameObject prefab, PopupEntryBase entry) where T : BasePopup
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

            if (instance.GetComponent<GraphicRaycaster>() == null)
                Debug.LogWarning($"[{GetType().Name}] Prefab '{prefab.name}' (PopupType.{entry.Type}) " +
                                 "has no GraphicRaycaster on the root object - the popup will not receive clicks.");

            return popup;
        }

        private bool TryTakeFromPool<T>(PopupEntryBase entry, out T popup) where T : BasePopup
        {
            popup = null;

            if (!_pool.TryGetValue(entry.Type, out BasePopup pooled))
                return false;

            _pool.Remove(entry.Type);

            if (!_instances.TryGetValue(pooled, out PopupInstance info))
            {
                if (pooled != null)
                    Object.Destroy(pooled.gameObject);

                return false;
            }

            if (pooled == null || pooled is not T typed)
            {
                DisposeInstance(pooled, info);
                return false;
            }

            pooled.gameObject.SetActive(true);
            pooled.transform.SetAsLastSibling();
            info.SortingOrder = AssignSortingOrder(pooled);

            popup = typed;
            return true;
        }

        private void DisposeInstance(BasePopup popup, PopupInstance info)
        {
            _instances.Remove(popup);

            if (popup != null)
                Object.Destroy(popup.gameObject);

            ReleasePrefab(info.Entry);
        }

        private int AssignSortingOrder(BasePopup popup)
        {
            if (!popup.TryGetComponent(out Canvas canvas))
            {
                Debug.LogWarning($"[{GetType().Name}] Popup '{popup.name}' has no Canvas " +
                                 "on the root object - sorting order was not applied.");
                return NoSortingOrder;
            }

            int order = _layersService.GetNextSortingOrder(UILayerType.Popup);
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
            return order;
        }

        private void ReleaseSortingOrder(PopupInstance info)
        {
            if (info.SortingOrder == NoSortingOrder)
                return;

            _layersService.ReleaseSortingOrder(UILayerType.Popup, info.SortingOrder);
            info.SortingOrder = NoSortingOrder;
        }

        #endregion

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
    }
}