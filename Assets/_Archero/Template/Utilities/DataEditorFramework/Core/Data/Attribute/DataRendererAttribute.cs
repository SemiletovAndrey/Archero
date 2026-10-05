using System;

namespace DataEditorFramework.Core.Rendering.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataRendererAttribute : System.Attribute
    {
        public Type TargetType { get; }

        public DataRendererAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}