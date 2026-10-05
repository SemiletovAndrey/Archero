using System;

namespace Template.Scripts.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class FormerNameAttribute : Attribute
    {
        public FormerNameAttribute(string value)
        {
            Value = value;
        }
        
        public string Value { get; }
    }
}