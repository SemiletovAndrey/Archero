using System.Collections.Generic;

namespace Template.Features.ScreenNavigator
{
    public class NavigationItemClosePayload
    {
        public NavigationItemClosePayload(List<BaseNavigationItem> openingItems)
        {
            OpeningItems = openingItems;
        }
        
        public List<BaseNavigationItem> OpeningItems { get; private set; }
    }
}