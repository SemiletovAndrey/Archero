using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public abstract class ActivatableScreen<TData> : BaseScreen where TData : BaseNavigationItemPayload
    {
        public override ScreenType ScreenType { get; protected set; }
        public abstract UniTask Activate(TData data);
        
        public abstract UniTask Open(NavigationItemOpenPayload payload);
        public abstract UniTask Close(NavigationItemClosePayload payload);
    }
}