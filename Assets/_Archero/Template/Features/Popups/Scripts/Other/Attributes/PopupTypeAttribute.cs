using System;

namespace Template.Features.Popups
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PopupTypeAttribute : Attribute
    {
        public PopupType Type { get; }
        public PopupTypeAttribute(PopupType type) => Type = type;
    }
}