using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Template.Features.ScreenNavigator
{
    public abstract class BaseNavigationItem
    {
        protected List<BaseNavigationItem> InnerItems { get; set; } = new();

        public IReadOnlyList<BaseNavigationItem> Children => InnerItems;
        
        public bool IsActive { get; protected set; }

        public virtual void Init()
        {
        }

        public List<BaseNavigationItem> GetItemPath<TItem>() where TItem : BaseNavigationItem
        {
            if (GetType() == typeof(TItem))
            {
                return new List<BaseNavigationItem> { this };
            }

            if (InnerItems.Count == 0)
            {
                return null;
            }

            foreach (BaseNavigationItem innerItem in InnerItems)
            {
                List<BaseNavigationItem> path = innerItem.GetItemPath<TItem>();

                if (path is not null)
                {
                    path.Insert(0, this);

                    return path;
                }
            }

            return null;
        }

        public TItem GetItem<TItem>() where TItem : BaseNavigationItem
        {
            if (GetType() == typeof(TItem))
            {
                return (TItem)this;
            }

            if (InnerItems.Count == 0)
            {
                return null;
            }

            foreach (BaseNavigationItem innerItem in InnerItems)
            {
                TItem item = innerItem.GetItem<TItem>();

                if (item is not null)
                {
                    return item;
                }
            }

            return null;
        }

        public virtual async UniTask MoveTo<TItem, TItemData>(TItemData data, NavigationItemOpenPayload payload)
            where TItem : BaseNavigationItem<TItemData>
            where TItemData : BaseNavigationItemPayload
        {
            if (GetType() == typeof(TItem))
            {
                return;
            }

            foreach (BaseNavigationItem innerItem in InnerItems)
            {
                if (innerItem.HasItem<TItem>())
                {
                    await innerItem.MoveTo<TItem, TItemData>(data, payload);
                    return;
                }
            }
        }

        public virtual UniTask Deactivate(NavigationItemClosePayload payload)
        {
            return UniTask.CompletedTask;
        }

        internal virtual UniTask Refresh(BaseNavigationItemPayload data)
        {
            return UniTask.CompletedTask;
        }

        private bool HasItem<TItem>() where TItem : BaseNavigationItem
        {
            if (GetType() == typeof(TItem))
            {
                return true;
            }

            foreach (var item in InnerItems)
            {
                if (item.HasItem<TItem>())
                {
                    return true;
                }
            }

            return false;
        }
    }
}