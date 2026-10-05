using System.Reflection;

namespace Template.Features.Cheat
{
    public class CheatCommandSettings
    {
        public string Name { get; init; }
        public string GroupName { get; init; }
        public MethodInfo Info { get; init; }
        public BaseCheatHandler Handler { get; init; }
    }
}
