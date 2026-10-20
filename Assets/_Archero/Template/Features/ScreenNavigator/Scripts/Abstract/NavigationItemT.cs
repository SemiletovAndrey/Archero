using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Scripts.Utils;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class BaseNavigationItem<TPayload> : BaseNavigationItem where TPayload : BaseNavigationItemPayload
    {
        private IClassFactory _classFactory;

        public event Func<TPayload, UniTask> Activated;

        public event Func<NavigationItemOpenPayload, UniTask> Opened;

        public event Func<NavigationItemClosePayload, UniTask> Closed;
        
        [Inject]
        private void Construct(IClassFactory classFactory)
        {
            _classFactory = classFactory;
        }

        public override void Init()
        {
            NavigationItemList itemList = AddInnerItems();
            CreateInnerItems(itemList);
        }

        protected virtual NavigationItemList AddInnerItems()
        {
            return new NavigationItemList();
        }

        public override async UniTask MoveTo<TItem, TItemData>(TItemData data, NavigationItemOpenPayload payload)
        {
            IsActive = true;

            if (GetType() == typeof(TItem))
            {
                TPayload openPayload = (TPayload)(object)data;
                await Activate(openPayload);
            }

            await OnOpen(payload);
            await Opened.InvokeAllAsync(payload);

            await base.MoveTo<TItem, TItemData>(data, payload);
        }

        public override async UniTask Deactivate(NavigationItemClosePayload payload)
        {
            try
            {
                await OnClose(payload);
                await Closed.InvokeAllAsync(payload);
                await base.Deactivate(payload);
            }
            finally
            {
                IsActive = false;
            }
        }

        public virtual async UniTask Activate(TPayload data)
        {
            await Activated.InvokeAllAsync(data);
        }

        protected virtual UniTask OnOpen(NavigationItemOpenPayload payload)
        {
            return UniTask.CompletedTask;
        }
        
        protected virtual UniTask OnClose(NavigationItemClosePayload payload)
        {
            return UniTask.CompletedTask;
        }

        internal override UniTask Refresh(BaseNavigationItemPayload data)
        {
            if (data is not TPayload typed)
                throw new InvalidCastException(
                    $"{GetType().Name} expects payload {typeof(TPayload).Name}, " +
                    $"got {(data == null ? "null" : data.GetType().Name)}.");

            return Activate(typed);
        }

        private void CreateInnerItems(NavigationItemList itemList)
        {
            InnerItems = itemList
                .Select(type =>
                {
                    BaseNavigationItem innerItem = (BaseNavigationItem)_classFactory.CreateAndBind(type);
                    innerItem.Init();

                    return innerItem;
                })
                .ToList();
        }
    }
}