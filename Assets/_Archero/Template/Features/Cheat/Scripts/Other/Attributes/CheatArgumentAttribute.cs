using System;

namespace Template.Features.Cheat
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class CheatArgumentAttribute : Attribute
    {
        public CheatArgumentAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}