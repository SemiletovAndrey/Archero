using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public abstract class ActivatableScreen<TData> : BaseScreen where TData : BaseNavigationItemPayload
    {
        public abstract UniTask Activate(TData data);
        
        public virtual UniTask Open(NavigationItemOpenPayload payload)
        {
            return UniTask.CompletedTask;
        }
        
        public virtual UniTask Close(NavigationItemClosePayload payload)
        {
            return UniTask.CompletedTask;
        }
    }
}