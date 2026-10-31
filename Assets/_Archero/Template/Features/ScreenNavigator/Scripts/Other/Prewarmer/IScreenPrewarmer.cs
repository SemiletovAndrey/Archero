using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Template.Features.ScreenNavigator
{
    public interface IScreenPrewarmer
    {
        public UniTask Prewarm(IEnumerable<ScreenType> screens, CancellationToken cancellationToken = default);
        
        public UniTask PrewarmChildrenOf(BaseNavigationItem item, CancellationToken cancellationToken = default);

    }
}