using System;
using System.Collections.Generic;
using System.Reflection;

namespace Template.Features.Popups
{
    public static class PopupTypeResolver
    {
        private static readonly Dictionary<Type, PopupType> Cache = new();

        public static PopupType Get<TPopup>() where TPopup : BasePopup
        {
            return Get(typeof(TPopup));
        }
        
        public static PopupType Get(Type popupClass)
        {
            if (!Cache.TryGetValue(popupClass, out PopupType popupType))
            {
                PopupTypeAttribute attribute = popupClass.GetCustomAttribute<PopupTypeAttribute>(inherit: false);
                popupType = attribute != null ? attribute.Type : PopupType.None;
                Cache[popupClass] = popupType;
            }

            return popupType;
        }

    }
}