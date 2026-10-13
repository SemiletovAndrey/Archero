using System;

namespace Template.Features.Popups
{
    public static class PopupEntryValidator
    {
        public static bool TryValidate(PopupEntryBase entry, out string error)
        {
            if (entry.Type == PopupType.None)
            {
                error = "Config has an entry with PopupType.None.";
                return false;
            }

            if (!entry.HasSource)
            {
                error = $"PopupType.{entry.Type} doesn't have a prefab assigned.";
                return false;
            }

            Type popupClass = entry.TryGetPopupClass();
            if (popupClass == null)
            {
                if (entry is PopupEntry)
                {
                    error = $"The prefab of PopupType.{entry.Type} has no {nameof(BasePopup)} component.";
                    return false;
                }
                
                error = null;
                return true;
            }

            PopupType declared = PopupTypeResolver.Get(popupClass);
            if (declared == PopupType.None)
            {
                error = $"{popupClass.Name} (PopupType.{entry.Type}) has no [{nameof(PopupTypeAttribute)}].";
                return false;
            }

            if (declared != entry.Type)
            {
                error = $"{popupClass.Name} declares PopupType.{declared} via [{nameof(PopupTypeAttribute)}], " +
                        $"but the entry is PopupType.{entry.Type}.";
                return false;
            }

            error = null;
            return true;
        }
    }
}