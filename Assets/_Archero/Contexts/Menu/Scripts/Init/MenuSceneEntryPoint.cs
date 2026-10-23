using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.GameStateMachine;

namespace Menu.Scripts.Init
{
    public class MenuSceneEntryPoint : ISceneEntryPoint
    {
        private readonly MenuFlow _flow;
        public MenuSceneEntryPoint(MenuFlow flow) => _flow = flow;

        public UniTask Enter(CancellationToken lifetime) => _flow.ShowMainMenu(lifetime);
    }
}