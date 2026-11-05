using System;

namespace Persistance.Features.LevelProgression
{
    public interface IChoiceLevelScreenView
    {
        public event Action BackClicked;
        
        public LevelTilesListView LevelTilesListView { get; }
        
        public void SetTitle(string title);
        public void SetInputBlocked(bool blocked);
    }
}