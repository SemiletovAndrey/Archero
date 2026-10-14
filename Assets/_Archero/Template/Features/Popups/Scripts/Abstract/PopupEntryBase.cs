namespace Template.Features.Popups
{
    public abstract class PopupEntryBase
    {
        public PopupType Type;
        
        public bool Pooled;
        
        public abstract bool HasSource { get; }
        
        public virtual System.Type TryGetPopupClass()
        {
            return null;
        }
    }
}