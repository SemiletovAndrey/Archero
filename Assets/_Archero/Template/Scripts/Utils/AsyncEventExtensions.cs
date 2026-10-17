using System;
using Cysharp.Threading.Tasks;

namespace Template.Scripts.Utils
{
    public static class AsyncEventExtensions
    {
        public static async UniTask InvokeAllAsync<T>(this Func<T, UniTask> handlers, T arg)
        {
            if (handlers == null)
                return;

            Delegate[] invocationList = handlers.GetInvocationList();
            for (int i = 0; i < invocationList.Length; i++)
            {
                await ((Func<T, UniTask>)invocationList[i]).Invoke(arg);
            }
        }

    }
}