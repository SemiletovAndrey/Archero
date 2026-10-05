using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Template.Features.ScreenNavigator;
using UnityEngine;
using Zenject;

namespace Template.Features.ProjectInit
{
    public abstract class BaseBootstrapper : MonoBehaviour
    {
        [SerializeField] private List<BaseInitializableBehaviour> _initBeforeInitialization;
        [SerializeField] private List<BaseInitializableBehaviour> _initAfterInitialization;
        
        [Inject]
        private void Construct(IScreenNavigator screenNavigator)
        {
            ScreenNavigator = screenNavigator;
        }
        
        protected IScreenNavigator ScreenNavigator { get; private set; }
        protected LinkedList<IInitializableFeature> Features { get; private set; }

        protected virtual BaseInstaller GetInstaller()
        {
            return GetComponent<BaseInstaller>();
        }
        
        protected virtual async UniTask DoBeforeSetup()
        {
            await UniTask.CompletedTask;
        }

        protected virtual async UniTask DoAfterSetup()
        {
            await UniTask.CompletedTask;
        }
        
        protected async UniTask StartInitialization()
        {
            InitBehavioursBeforeSetup();

            await DoBeforeSetup();
            
            SetupFeatures();

            await InitFeatures();
            await DoAfterSetup();
            
            InitBehavioursAfterSetup();
        }

        private void InitBehavioursBeforeSetup()
        {
            _initBeforeInitialization.ForEach(behaviour => behaviour.Init());
        }
        
        private void InitBehavioursAfterSetup()
        {
            _initAfterInitialization.ForEach(behaviour => behaviour.Init());
        }
        
        private void SetupFeatures()
        {
            BaseInstaller installer = GetInstaller();
            Features = installer.GetFeatures();
        }
        
        private async UniTask InitFeatures()
        {
            foreach (IInitializableFeature feature in Features)
            {
                try
                {
                    await feature.InitFeature();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}