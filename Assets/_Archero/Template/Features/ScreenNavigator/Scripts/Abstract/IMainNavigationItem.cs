using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Template.Features.ScreenNavigator
{
    public interface IMainNavigationItem
    {
        public void Init();

        public List<BaseNavigationItem> GetItemPath<TItem>() where TItem : BaseNavigationItem;
        public UniTask MoveTo<TItem, TItemData>(TItemData data, NavigationItemOpenPayload payload)
            where TItem : BaseNavigationItem<TItemData>
            where TItemData : BaseNavigationItemPayload;
    }
}