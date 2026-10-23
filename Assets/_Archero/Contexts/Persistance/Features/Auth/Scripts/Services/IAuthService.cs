using System;
using Cysharp.Threading.Tasks;
using Template.Scripts.DataHolders;
using Template.Utilities.Primitives;

namespace Persistence.Features.Auth
{
    public interface IAuthService
    {
        public event Action<UserData> LoggedIn;
        public event Action LogOutUser;
        
        public IModifierCollection<UserData> UserLoggedIn { get; }

        public UniTask InitAsync();

        public UniTask<Result> SignInEmailAsync(Email email, Password password);
        public UniTask<Result> RegisterEmailAsync(Email email, UserName userName, Password password, int age);

        public void LogOut();
    }
}