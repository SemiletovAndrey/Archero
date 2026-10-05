using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DomainOfBattles.Contexts.Preloader
{
    public class ProjectBootstrapper : BaseProjectBootstrapper
    {
        protected override UniTask DoBeforeSetup()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            return UniTask.CompletedTask;
        }

        protected override async UniTask DoAfterSetup()
        {
            
        }
    }
}
