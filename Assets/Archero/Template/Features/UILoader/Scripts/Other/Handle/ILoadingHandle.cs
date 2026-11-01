using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.UILoader
{
    public interface ILoadingHandle : IDisposable
    {
        public void SetProgress(float progress);

        public void SetMessage(string message);

        public UniTask Hide();

    }
}