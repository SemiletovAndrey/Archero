using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.GameFlow.Scripts;
using Template.Features.NewInputFeature;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Template.Features.SceneLoader
{
    public class SceneLoader : ISceneLoader, IDisposable
    {
        private readonly ZenjectSceneLoader _zenjectSceneLoader;
        private readonly IFeatureInitializer _featureInitializer;
        private readonly IInputProviderManager _inputProviderManager;

        public SceneLoader(ZenjectSceneLoader zenjectSceneLoader, IFeatureInitializer featureInitializer, IInputProviderManager inputProviderManager)
        {
            _zenjectSceneLoader = zenjectSceneLoader;
            _featureInitializer = featureInitializer;
            _inputProviderManager = inputProviderManager;
        }
        
        public void Dispose()
        {
            
        }

        public UniTask<LoadedScene> Load(string sceneName, CancellationToken ct)
        {
            return LoadInternal(sceneName, null, ct);
        }

        public UniTask<LoadedScene> Load<TPayload>(string sceneName, TPayload payload, CancellationToken ct)
            where TPayload : class
        {
            return LoadInternal(sceneName, container => container.BindInstance(payload), ct);
        }

        public async UniTask Release(LoadedScene scene)
        {
            await _featureInitializer.DeinitAll(scene.Features);
        }

        private async UniTask<LoadedScene> LoadInternal(
            string sceneName, Action<DiContainer> extraBindings, CancellationToken ct)
        {
            _inputProviderManager.ClearSceneProviders();
            
            AsyncOperation operation =
                _zenjectSceneLoader.LoadSceneAsync(sceneName, LoadSceneMode.Single, extraBindings);
            
            await operation.ToUniTask(cancellationToken: ct);

            Scene scene = SceneManager.GetSceneByName(sceneName);
            SceneContext sceneContext = FindSceneContext(scene);

            if (sceneContext == null || sceneContext.Container == null)
            {
                throw new InvalidOperationException(
                    $"Scene '{sceneName}' has no installed SceneContext. Add one to the scene.");
            }

            DiContainer container = sceneContext.Container;

            if (!container.HasBinding<SceneFeatureSet>())
            {
                throw new InvalidOperationException(
                    $"Scene '{sceneName}' does not bind SceneFeatureSet. Use an installer derived from SceneInstaller.");
            }

            return new LoadedScene(scene, container, container.Resolve<SceneFeatureSet>());
        }

        private static SceneContext FindSceneContext(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                SceneContext context = root.GetComponentInChildren<SceneContext>(true);

                if (context != null)
                {
                    return context;
                }
            }

            return null;
        }
    }
}