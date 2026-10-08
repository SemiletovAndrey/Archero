using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.GameFlow.Scripts.Other.FeatureInitializer;
using UnityEngine;

namespace Template.Features.GameStateMachine
{
    public class BootState : IState
    {
        private readonly IGameStateMachine _stateMachine;
        private readonly IFeatureInitializer _featureInitializer;
        private readonly GlobalBaseFeatureSet _globalBaseFeatures;
 
        public BootState(
            IGameStateMachine stateMachine, IFeatureInitializer featureInitializer, GlobalBaseFeatureSet globalBaseFeatures)
        {
            _stateMachine = stateMachine;
            _featureInitializer = featureInitializer;
            _globalBaseFeatures = globalBaseFeatures;
        }
 
        public async UniTask Enter(CancellationToken lifetime)
        {
            ApplyApplicationSettings();
            
            await _featureInitializer.InitAll(_globalBaseFeatures, lifetime);
            
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