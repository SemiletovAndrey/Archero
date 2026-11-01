using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.Popups
{
    public abstract class BasePopupT<TPayload> : BasePopup where TPayload : IPopupPayload
    {
        internal override Type PayloadType => typeof(TPayload);
        public abstract UniTask Activate(TPayload payload);
    }
}