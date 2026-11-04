using Cysharp.Threading.Tasks;
using Template.Features.ScreenNavigator;
using TMPro;
using UnityEngine;

namespace Persistance.Features.LevelProgression
{
    public class ChoiceLevelScreenView : AnimatedActivatableScreen<ChoiceLevelPayload>, IChoiceLevelScreenView
    {
        [SerializeField]  private TextMeshProUGUI _titleText;
        [SerializeField]  private LevelTilesListView _levelTilesListView;
        
        public LevelTilesListView LevelTilesListView => _levelTilesListView;
        
        public override UniTask Activate(ChoiceLevelPayload data)
        {
            return UniTask.CompletedTask;
        }
        
        public void SetTitle(string title) =>  _titleText.text = title;
        
        
    }
}