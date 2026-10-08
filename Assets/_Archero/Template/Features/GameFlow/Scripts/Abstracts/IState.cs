using System.Threading;
using Cysharp.Threading.Tasks;

namespace Template.Features.GameStateMachine
{
    public interface IState : IExitableState
    {
        public UniTask Enter(CancellationToken ct);
    }
}