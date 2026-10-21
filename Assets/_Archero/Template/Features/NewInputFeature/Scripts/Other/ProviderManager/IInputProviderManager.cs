using System;

namespace Template.Features.NewInputFeature
{
    public interface IInputProviderManager : IDisposable
    {
        public TProvider CreateProvider<TProvider>() where TProvider : IInputProvider;
        
        public TProvider GetProvider<TProvider>() where TProvider : class, IInputProvider;
        
        public void EnableProvider<TProvider>() where TProvider : IInputProvider;
        public void DisableProvider<TProvider>() where TProvider : IInputProvider;
        
        public void RemoveProvider<TProvider>() where TProvider : IInputProvider;

        public void EnableAll();
        public void DisableAll();
        
        public void ClearSceneProviders();
    }
}