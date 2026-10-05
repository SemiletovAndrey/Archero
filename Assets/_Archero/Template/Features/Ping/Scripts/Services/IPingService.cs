using System;
using Cysharp.Threading.Tasks;

namespace Template.Features.Ping
{
    public interface IPingService
    {
        public event Action OnInternetRestored;
        public event Action OnInternetLost;
        public event Action<ConnectionState> OnStateChanged;
        
        public bool HasConnection();
        public UniTask WaitForOnline();
        
        public void AddProvider(IPingProvider provider);
        public void RemoveProvider(IPingProvider provider);
    }
}