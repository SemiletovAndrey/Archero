using System;

namespace Template.Scripts.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AbstractMemberAttribute : Attribute
    {
        public Type EnumType { get; }
        public long EnumValue { get; }
        public string StringValue { get; }
        
        public AbstractMemberAttribute(object value)
        {
            EnumType = value.GetType();
            EnumValue = Convert.ToInt64(value);
        }
        
        public AbstractMemberAttribute(string value)
        {
            StringValue = value;
        }
    }
}