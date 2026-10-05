using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public static class NavigationItemUtils
    {
        public static bool HasItem(this List<BaseNavigationItem> items, Type type)
        {
            foreach (BaseNavigationItem item in items)
            {
                Type itemType = item.GetType();

                while (itemType != null && itemType != typeof(object))
                {
                    if (type.IsGenericTypeDefinition)
                    {
                        if (itemType.IsGenericType && itemType.GetGenericTypeDefinition() == type)
                            return true;
                    }

                    else if (itemType == type)
                    {
                        return true;
                    }

                    itemType = itemType.BaseType;
                }
            }

            return false;
        }
        
        public static bool HasItem<T>(this List<BaseNavigationItem> items)
        {
            return items.OfType<T>().Any();
        }
    }
}