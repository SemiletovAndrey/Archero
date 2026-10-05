using System;
using Cysharp.Threading.Tasks;

namespace Template.Scripts.DataHolders
{
    public interface IAsyncEvent
    {
        public void Subscribe(Func<UniTask> callback);
        public void Unsubscribe(Func<UniTask> callback);
    }

    public interface IAsyncEvent<T>
    {
        public void Subscribe(Func<T, UniTask> callback);
        public void Unsubscribe(Func<T, UniTask> callback);
    }
}