using System.Threading;
using Cysharp.Threading.Tasks;

namespace Template.Features.GameStateMachine
{
    public interface IPayloadState<TPayload> : IExitableState
    {
        public UniTask Enter(TPayload payload, CancellationToken lifetime);
    }
}