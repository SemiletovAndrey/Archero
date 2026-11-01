using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Template.Scripts.DataHolders
{
    public class AsyncEvent : IAsyncEvent
    {
        private LinkedList<Func<UniTask>> _callbacks = new();
        
        public void Subscribe(Func<UniTask> callback)
        {
            _callbacks.AddLast(callback);
        }

        public void Unsubscribe(Func<UniTask> callback)
        {
            _callbacks.Remove(callback);
        }

        public async UniTask Invoke(bool parallel = false)
        {
            if (parallel)
            {
                await UniTask.WhenAll(_callbacks.Select(subscriber => subscriber()));
            }
            else
            {
                foreach (Func<UniTask> subscriber in _callbacks)
                {
                    await subscriber();
                }
            }
        }
    }

    public class AsyncEvent<T> : IAsyncEvent<T>
    {
        private LinkedList<Func<T, UniTask>> _callbacks = new();
        
        public void Subscribe(Func<T, UniTask> callback)
        {
            _callbacks.AddLast(callback);
        }

        public void Unsubscribe(Func<T, UniTask> callback)
        {
            _callbacks.Remove(callback);
        }

        public async UniTask Invoke(T param,bool parallel = false)
        {
            if (parallel)
            {
                await UniTask.WhenAll(_callbacks.Select(subscriber => subscriber(param)));
            }
            else
            {
                foreach (Func<T, UniTask> subscriber in _callbacks)
                {
                    await subscriber(param);
                }
            }
        }
    }
}