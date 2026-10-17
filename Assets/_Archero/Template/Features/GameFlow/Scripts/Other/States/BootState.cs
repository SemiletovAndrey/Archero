using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.GameFlow.Scripts;
using UnityEngine;

namespace Template.Features.GameStateMachine
{
    public class BootState : IState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IFeatureInitializer _featureInitializer;
        private readonly GlobalFeatureSet _globalFeatures;
 
        public BootState(
            IGameStateMachine stateMachine, IFeatureInitializer featureInitializer, GlobalFeatureSet globalFeatures)
        {
            _stateMachine = stateMachine;
            _featureInitializer = featureInitializer;
            _globalFeatures = globalFeatures;
        }
 
        public async UniTask Enter(CancellationToken lifetime)
        {
            ApplyApplicationSettings();
            
            await _featureInitializer.InitAll(_globalFeatures, lifetime);
            
            _stateMachine.Enter<MenuState>();
        }
 
        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
 
        private static void ApplyApplicationSettings()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }
    }
}