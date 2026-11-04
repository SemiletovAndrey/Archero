using Cysharp.Threading.Tasks;
using Template.Features.Presenters;

namespace Persistance.Features.LevelProgression
{
    public class ChoiceLevelScreenPresenter : IScreenPresenter<ChoiceLevelPayload>
    {
        public UniTask Activate(ChoiceLevelPayload payload)
        {
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            
        }
    }
}