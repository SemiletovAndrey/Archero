using Cysharp.Threading.Tasks;
using Firebase.Auth;

namespace Template.Features.Firebase.Services
{
    public interface IFirebaseService
    {
        public bool IsFirebaseInitialized { get; }
        public FirebaseAuth Auth { get; }

        public UniTask InitAsync();
    }
}