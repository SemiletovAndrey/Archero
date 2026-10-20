using System.Threading;
using Cysharp.Threading.Tasks;

namespace Template.Features.GameStateMachine
{
    public interface ISceneEntryPoint
    {
        public UniTask Enter(CancellationToken lifetime);
    }
}