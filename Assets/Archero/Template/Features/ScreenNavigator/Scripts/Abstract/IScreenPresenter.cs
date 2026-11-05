using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.Presenters
{
    public interface IScreenPresenter<in TPayload> : IDisposable
    {
        public void Init();
        public UniTask Activate(TPayload payload);
    }
}