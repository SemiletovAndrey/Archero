using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.UIViewFactory
{
    public interface IUIViewFactory<TView, TEntry>
        where TView : Component
        where TEntry : class
    {
        public void Init(Transform parent);

        public UniTask<T> Create<T>(TEntry entry, Transform parent = null,
            CancellationToken cancellationToken = default)
            where T : TView;

        public void Release(TView view);

        public UniTask Preload(TEntry entry, CancellationToken cancellationToken = default);

        public UniTask Prewarm(TEntry entry, CancellationToken cancellationToken = default);

        public void Unload(TEntry entry);
    }
}