using System;

namespace Template.Features.ScreenNavigator
{
    [Serializable]
    public abstract class ScreenEntryBase
    {
        public ScreenType Type;
        
        public bool Pooled = true;
        
        public abstract bool HasSource { get; }

    }
}