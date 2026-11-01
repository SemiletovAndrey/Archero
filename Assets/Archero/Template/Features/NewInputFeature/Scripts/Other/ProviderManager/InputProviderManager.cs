using System;
using System.Collections.Generic;
using Template.Features.ClassFactory;
using UnityEngine;

namespace Template.Features.NewInputFeature
{
    public class InputProviderManager : IInputProviderManager
    {
        private readonly IClassFactory _classFactory;
        private readonly Dictionary<Type, IInputProvider> _providers = new();

        public InputProviderManager(IClassFactory classFactory)
        {
            _classFactory = classFactory;
        }

        public TProvider CreateProvider<TProvider>() where TProvider : IInputProvider
        {
            var type = typeof(TProvider);
            if (_providers.ContainsKey(type))
            {
                return (TProvider)_providers[type];
            }
            
            TProvider provider = _classFactory.Create<TProvider>();

            _providers[type] = provider;

            return provider;
        }

        public TProvider GetProvider<TProvider>() where TProvider : class, IInputProvider
        {
            return _providers.TryGetValue(typeof(TProvider), out var provider) ? provider as TProvider : null;
        }

        public void EnableProvider<TProvider>() where TProvider : IInputProvider
        {
            if (_providers.TryGetValue(typeof(TProvider), out var provider))
                provider.Enable();
        }

        public void DisableProvider<TProvider>() where TProvider : IInputProvider
        {
            if (_providers.TryGetValue(typeof(TProvider), out var provider))
                provider.Disable();
        }

        public void RemoveProvider<TProvider>() where TProvider : IInputProvider
        {
            var type = typeof(TProvider);
            if (_providers.TryGetValue(type, out var provider))
            {
                provider.Disable();
                (provider as IDisposable)?.Dispose();
                _providers.Remove(type);
            }
        }

        public void EnableAll()
        {
            foreach (var provider in _providers.Values)
                provider.Enable();
        }

        public void DisableAll()
        {
            foreach (var provider in _providers.Values)
                provider.Disable();
        }

        public void ClearSceneProviders()
        {
            Debug.Log("Clearing scene providers");
            
            foreach (var provider in _providers.Values)
            {
                provider.Disable();
                (provider as IDisposable)?.Dispose();
            }
            _providers.Clear();
        }

        public void Dispose()
        {
            foreach (var provider in _providers.Values)
            {
                provider.Disable();
                (provider as IDisposable)?.Dispose();
            }

            _providers.Clear();
        }
    }
}