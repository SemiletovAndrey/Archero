using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.GameStateMachine;
using Template.Features.UILoader;

namespace Menu.Scripts.Init
{
    public class MenuSceneEntryPoint : ISceneEntryPoint
    {
        private readonly MenuFlow _flow;
        private readonly ILoadingSystem _loadingSystem;

        public MenuSceneEntryPoint(MenuFlow flow, ILoadingSystem loadingSystem)
        {
            _flow = flow;
            _loadingSystem = loadingSystem;
        }

        public async UniTask Enter(CancellationToken lifetime)
        {
            try
            {
                await _flow.ShowMainMenu(lifetime);
            }
            finally
            {
                await _loadingSystem.Hide();
            }
        }
    }
}