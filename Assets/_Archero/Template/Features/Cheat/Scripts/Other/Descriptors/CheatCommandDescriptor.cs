using System.Collections.Generic;

namespace Template.Features.Cheat
{
    public class CheatCommandDescriptor
    {
        public string Name { get; init; }
        public string GroupName { get; init; }
        public IReadOnlyList<CheatArgumentDescriptor> Arguments { get; init; }
    }
}
