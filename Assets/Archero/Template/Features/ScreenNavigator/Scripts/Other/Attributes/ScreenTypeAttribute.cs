using System;

namespace Template.Features.ScreenNavigator
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ScreenTypeAttribute : Attribute
    {
        public ScreenType Type { get; }
        public ScreenTypeAttribute(ScreenType type) => Type = type;
    }
}