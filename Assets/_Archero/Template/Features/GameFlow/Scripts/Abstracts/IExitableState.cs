using Cysharp.Threading.Tasks;

namespace Template.Features.GameStateMachine
{
    public interface IExitableState
    {
        public UniTask Exit();
    }
}