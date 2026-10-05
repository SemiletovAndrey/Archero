using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class NavigationItemActivator<TItem, TData> : MonoBehaviour
        where TItem : BaseNavigationItem<TData>
        where TData : BaseNavigationItemPayload
    {
        private TItem _navigationItem;
        
        [Inject]
        private void Construct(TItem item)
        {
            _navigationItem = item;
        }
        
        protected virtual void Awake()
        {
            _navigationItem.Opened += Open;
            _navigationItem.Closed += Close;
            _navigationItem.Initialized += Activate;
        }

        private void OnDestroy()
        {
            _navigationItem.Opened -= Open;
            _navigationItem.Closed -= Close;
            _navigationItem.Initialized -= Activate;
        }

        protected abstract UniTask Activate(TData data);
        protected abstract UniTask Open(NavigationItemOpenPayload payload);
        protected abstract UniTask Close(NavigationItemClosePayload payload);
    }
}