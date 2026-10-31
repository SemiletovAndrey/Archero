using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.UILoader
{
    public interface ILoadingSystem
    {
        public bool IsVisible { get; }
        
        public UniTask Show(string message = null);
        
        public UniTask Hide();
        
        public UniTask HideAll();
        
        public UniTask<ILoadingHandle> Acquire(string message = null);

        public UniTask Run(Func<UniTask> work, string message = null);

        public UniTask<T> Run<T>(Func<UniTask<T>> work, string message = null);
        UniTask ShowWithoutAnimation(string message = null);
    }
}