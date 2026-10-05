using Cysharp.Threading.Tasks;

namespace Template.Features.Popups
{
    public abstract class BasePopup<TPayload> : BasePopup where TPayload : IPopupPayload
    {
        public abstract UniTask Activate(TPayload payload);
    }
}