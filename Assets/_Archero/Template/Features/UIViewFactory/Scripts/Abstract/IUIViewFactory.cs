using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.UIViewFactory
{
    public interface IUIViewFactory<TView, TEntry>
        where TView : Component
        where TEntry : class
    {
        /// <summary>Родитель по умолчанию, в который создаются экземпляры.</summary>
        public void Init(Transform parent);

        /// <summary>
        /// Создает представление (или берет из пула). Если parent не указан, используется родитель из Init.
        /// </summary>
        public UniTask<T> Create<T>(TEntry entry, Transform parent = null) where T : TView;

        /// <summary>Представление больше не нужно: фабрика уничтожает его или возвращает в пул.</summary>
        public void Release(TView view);

        /// <summary>Загружает ресурсы заранее и удерживает их до Unload (если источник это поддерживает).</summary>
        public UniTask Preload(TEntry entry);

        /// <summary>Уничтожает экземпляр из пула и снимает удержание ресурсов. Живые экземпляры не затрагиваются.</summary>
        public void Unload(TEntry entry);

    }
}