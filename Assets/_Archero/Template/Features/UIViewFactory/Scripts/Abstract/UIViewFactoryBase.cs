using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using UnityEngine;
using UnityEngine.UI;

namespace Template.Features.UIViewFactory
{
    public abstract class UIViewFactoryBase<TView, TEntry> : IUIViewFactory<TView, TEntry>
        where TView : Component
        where TEntry : class
    {
        private const int NoSortingOrder = -1;

        private sealed class ViewInstance
        {
            public readonly TEntry Entry;
            public int SortingOrder = NoSortingOrder;

            public ViewInstance(TEntry entry)
            {
                Entry = entry;
            }
        }

        private Transform _parent;

        private readonly IClassFactory _classFactory;
        private readonly IUILayersService _layersService;

        // Все живые экземпляры, включая спрятанные в пуле.
        private readonly Dictionary<TView, ViewInstance> _instances = new();

        // По одному спрятанному экземпляру на ключ: сейчас оба потребителя держат не больше одного экземпляра на тип.
        private readonly Dictionary<int, TView> _pool = new();

        protected UIViewFactoryBase(IClassFactory classFactory, IUILayersService layersService)
        {
            _classFactory = classFactory;
            _layersService = layersService;
        }

        #region Что задает наследник

        /// <summary>Слой, из диапазона которого выдается Sort Order.</summary>
        protected abstract UILayerType Layer { get; }

        /// <summary>Ключ пула. Фабрика хранит свой пул, поэтому достаточно уникальности внутри нее (обычно (int)Type).</summary>
        protected abstract int GetKey(TEntry entry);

        protected abstract bool IsPooled(TEntry entry);

        protected abstract bool HasSource(TEntry entry);

        /// <summary>Короткое описание записи для сообщений об ошибках, например PopupType.Loader.</summary>
        protected abstract string Describe(TEntry entry);

        /// <summary>
        /// Возвращает префаб и берет на него использование, которое держится до ReleasePrefab.
        /// Если метод бросил исключение, наследник сам ничего не удерживает.
        /// </summary>
        protected abstract UniTask<GameObject> AcquirePrefab(TEntry entry);

        /// <summary>Экземпляр уничтожен: снимаем использование, взятое в AcquirePrefab.</summary>
        protected abstract void ReleasePrefab(TEntry entry);

        /// <summary>Вызывается после создания нового экземпляра (Sort Order уже выдан).</summary>
        protected virtual void OnViewCreated(TView view, TEntry entry)
        {
        }

        /// <summary>Вызывается, когда экземпляр взят из пула (Sort Order уже выдан).</summary>
        protected virtual void OnViewReused(TView view, TEntry entry)
        {
        }

        #endregion

        public void Init(Transform parent)
        {
            _parent = parent;
        }

        public virtual UniTask Preload(TEntry entry)
        {
            return UniTask.CompletedTask;
        }

        public virtual void Unload(TEntry entry)
        {
            if (entry == null || !_pool.TryGetValue(GetKey(entry), out TView pooled))
                return;

            _pool.Remove(GetKey(entry));

            if (_instances.TryGetValue(pooled, out ViewInstance info))
                DisposeInstance(pooled, info);
            else if (!IsDestroyed(pooled))
                Object.Destroy(pooled.gameObject);
        }

        public void Init(RuleTile.TilingRuleOutput.Transform parent)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<T> Create<T>(TEntry entry, Transform parent = null) where T : TView
        {
            Transform target = ResolveParent(parent);
            EnsureSource(entry);

            if (IsPooled(entry) && TryTakeFromPool(entry, target, out T pooled))
                return pooled;

            GameObject prefab = await AcquirePrefab(entry);

            T view = null;
            ViewInstance info = null;
            try
            {
                view = InstantiateView<T>(prefab, entry, target);

                info = new ViewInstance(entry);
                _instances[view] = info;
                info.SortingOrder = AssignSortingOrder(view);

                OnViewCreated(view, entry);
                return view;
            }
            catch
            {
                if (info != null)
                    ReleaseSortingOrder(info);

                if (view != null)
                {
                    _instances.Remove(view);

                    if (!IsDestroyed(view))
                        Object.Destroy(view.gameObject);
                }

                ReleasePrefab(entry);
                throw;
            }
        }

        public void Release(TView view)
        {
            if (ReferenceEquals(view, null))
                return;

            if (!_instances.TryGetValue(view, out ViewInstance info))
            {
                // Экземпляр создан не этой фабрикой.
                if (!IsDestroyed(view))
                    Object.Destroy(view.gameObject);

                return;
            }

            ReleaseSortingOrder(info);

            int key = GetKey(info.Entry);
            if (IsPooled(info.Entry) && !IsDestroyed(view) && !_pool.ContainsKey(key))
            {
                // В пул: объект выключен, а использование префаба остается занятым.
                view.gameObject.SetActive(false);
                _pool[key] = view;
                return;
            }

            DisposeInstance(view, info);
        }

        #region Instances

        private T InstantiateView<T>(GameObject prefab, TEntry entry, Transform parent) where T : TView
        {
            TView instance = _classFactory.CreateMonoBehaviour<TView>(prefab, parent);
            if (IsDestroyed(instance))
                throw new System.InvalidOperationException(
                    $"{Describe(entry)}: failed to create instance from prefab '{prefab.name}'.");

            if (!instance.TryGetComponent(out T view))
            {
                Object.Destroy(instance.gameObject);
                throw new System.InvalidOperationException(
                    $"Prefab '{prefab.name}' ({Describe(entry)}) has no component {typeof(T).Name}.");
            }

            if (instance.GetComponent<GraphicRaycaster>() == null)
                Debug.LogWarning($"[{GetType().Name}] Prefab '{prefab.name}' ({Describe(entry)}) " +
                                 "has no GraphicRaycaster on the root object - it will not receive clicks.");

            return view;
        }

        private bool TryTakeFromPool<T>(TEntry entry, Transform parent, out T view) where T : TView
        {
            view = null;

            int key = GetKey(entry);
            if (!_pool.TryGetValue(key, out TView pooled))
                return false;

            _pool.Remove(key);

            if (!_instances.TryGetValue(pooled, out ViewInstance info))
            {
                if (!IsDestroyed(pooled))
                    Object.Destroy(pooled.gameObject);

                return false;
            }

            // Экземпляр мог быть уничтожен снаружи (например, вместе со сценой) или не подойти по типу:
            // тогда освобождаем его запись и создаем новый.
            if (IsDestroyed(pooled) || pooled is not T typed)
            {
                DisposeInstance(pooled, info);
                return false;
            }

            if (pooled.transform.parent != parent)
                pooled.transform.SetParent(parent, false);

            pooled.gameObject.SetActive(true);
            pooled.transform.SetAsLastSibling();
            info.SortingOrder = AssignSortingOrder(pooled);

            OnViewReused(pooled, info.Entry);

            view = typed;
            return true;
        }

        private void DisposeInstance(TView view, ViewInstance info)
        {
            _instances.Remove(view);

            // Сначала уничтожаем экземпляр, потом отпускаем ресурсы.
            if (!IsDestroyed(view))
                Object.Destroy(view.gameObject);

            ReleasePrefab(info.Entry);
        }

        private int AssignSortingOrder(TView view)
        {
            if (!view.TryGetComponent(out Canvas canvas))
            {
                Debug.LogWarning($"[{GetType().Name}] '{view.name}' has no Canvas on the root object - " +
                                 "sorting order was not applied.");
                return NoSortingOrder;
            }

            int order = _layersService.GetNextSortingOrder(Layer);
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
            return order;
        }

        private void ReleaseSortingOrder(ViewInstance info)
        {
            if (info.SortingOrder == NoSortingOrder)
                return;

            _layersService.ReleaseSortingOrder(Layer, info.SortingOrder);
            info.SortingOrder = NoSortingOrder;
        }

        #endregion

        protected void EnsureSource(TEntry entry)
        {
            if (entry == null)
                throw new System.ArgumentNullException(nameof(entry));

            if (!HasSource(entry))
                throw new System.InvalidOperationException($"{Describe(entry)}: prefab source is not assigned.");
        }

        private Transform ResolveParent(Transform parent)
        {
            Transform target = parent != null ? parent : _parent;
            if (target == null)
                throw new System.InvalidOperationException(
                    $"{GetType().Name} is not initialized. Call Init(parent) first or pass a parent to Create.");

            return target;
        }

        // Явное приведение к UnityEngine.Object гарантирует проверку "уничтожен ли объект" в generic-коде.
        protected static bool IsDestroyed(Object unityObject)
        {
            return unityObject == null;
        }

    }
}