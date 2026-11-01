using Cysharp.Threading.Tasks;

namespace Template.Features.Popups
{
    public abstract class BasePopupTR<TPayload, TResult> : BasePopupT<TPayload> where TPayload : IPopupPayload
    {
        private UniTaskCompletionSource<TResult> _result = new();

        internal UniTask<TResult> ResultTask => _result.Task;
        
        protected void Complete(TResult result)
        {
            _result.TrySetResult(result);
            CloseExternal();
        }

        internal override void NotifyOpening()
        {
            _result = new UniTaskCompletionSource<TResult>();
        }

        internal override void NotifyClosed()
        {
            _result.TrySetResult(default);
        }
    }
}