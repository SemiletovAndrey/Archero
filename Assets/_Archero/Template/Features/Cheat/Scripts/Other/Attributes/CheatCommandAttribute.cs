using System;

namespace Template.Features.Cheat
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CheatCommandAttribute : Attribute
    {
        public CheatCommandAttribute(string name, string groupName = "General")
        {
            Name = name;
            GroupName = string.IsNullOrWhiteSpace(groupName) ? "General" : groupName;
        }

        public string Name { get; }
        public string GroupName { get; }
    }
}
