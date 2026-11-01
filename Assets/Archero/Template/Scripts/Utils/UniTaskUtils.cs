using System;
using Cysharp.Threading.Tasks;

namespace Template.Scripts.Utils
{
    public static class UniTaskUtils
    {
        public static UniTask WaitUntilOrPass(Func<bool> predicate)
        {
            return predicate() ? UniTask.CompletedTask : UniTask.WaitUntil(predicate);
        }
    }
}