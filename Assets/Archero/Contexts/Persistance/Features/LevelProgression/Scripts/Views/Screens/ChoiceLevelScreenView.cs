using Cysharp.Threading.Tasks;
using Template.Features.ScreenNavigator;

namespace Persistance.Features.LevelProgression
{
    public class ChoiceLevelScreenView : AnimatedActivatableScreen<ChoiceLevelPayload>, IChoiceLevelScreenView
    {
        public override UniTask Activate(ChoiceLevelPayload data)
        {
            return UniTask.CompletedTask;
        }
    }
}