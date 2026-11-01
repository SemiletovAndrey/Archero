using System.Collections.Generic;

namespace Template.Features.ScreenNavigator
{
    public class NavigationItemOpenPayload
    {
        public NavigationItemOpenPayload(List<BaseNavigationItem> closingItems)
        {
            ClosingItems = closingItems;
        }
        
        public List<BaseNavigationItem> ClosingItems { get; private set; }
    }
}