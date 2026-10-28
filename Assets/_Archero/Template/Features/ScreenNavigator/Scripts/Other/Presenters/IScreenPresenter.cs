using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.ScreenNavigator
{
    public interface IScreenPresenter<in TPayload> : IDisposable
    {
        public UniTask Activate(TPayload payload);
    }
}