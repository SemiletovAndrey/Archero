using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class BaseNavigationItem<TPayload> : BaseNavigationItem where TPayload : BaseNavigationItemPayload
    {
        private IClassFactory _classFactory;
        
        public event Func<TPayload, UniTask> Initialized;
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
            if (GetType() == typeof(TItem))
            {
                TPayload openPayload = (TPayload)(object)data;
                await Activate(openPayload);
            }

            if (Opened is not null)
            {
                await Opened.Invoke(payload);
            }
            
            await base.MoveTo<TItem, TItemData>(data, payload);
        }

        public override async UniTask Deactivate(NavigationItemClosePayload payload)
        {
            if (Closed is not null)
            {
                await Closed.Invoke(payload);
            }
            
            await base.Deactivate(payload);
        }

        public virtual async UniTask Activate(TPayload data)
        {
            if (Initialized is not null)
            {
                await Initialized.Invoke(data);
            }
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