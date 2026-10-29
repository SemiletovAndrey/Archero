using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.Presenters
{
    public interface IPresenter<in TPayload> : IDisposable
    {
        public UniTask Activate(TPayload payload);
    }
}