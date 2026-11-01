using System.Collections.Generic;
using UnityEngine;

namespace Template.Features.Container
{
    public class ContainerSystem : IContainerSystem
    {
        private List<IContainerProvider> _providers = new();
        
        public void Init()
        {
        }
        
        public TContainer GetContainer<TContainer>() where TContainer : IContainer
        {
            if (_providers.Count == 0)
            {
                Debug.LogError("No container providers registered in ContainerSystem.");
                //var allProviders = GameObject.FindObjectsByType<SceneContainerProvider>(FindObjectsSortMode.None);

                //_providers.AddRange(allProviders);
            }

            foreach (IContainerProvider provider in _providers)
            {
                if (provider.TryGet(out TContainer container))
                {
                    return container;
                }
            }

            Debug.LogError($"Container of type {typeof(TContainer)} not found in any provider.");
            return default;
        }

        public void AddProvider(IContainerProvider provider)
        {
            _providers.Add(provider);
        }

        public void RemoveProvider(IContainerProvider provider)
        {
            _providers.Remove(provider);
        }
    }
}