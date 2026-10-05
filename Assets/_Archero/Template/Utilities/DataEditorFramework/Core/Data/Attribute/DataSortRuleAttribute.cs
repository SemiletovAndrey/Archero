using System;

namespace DataEditorFramework.Core.Data.Attribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DataSortRuleAttribute : System.Attribute
    {
        public Type TargetType { get; }

        public DataSortRuleAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}
