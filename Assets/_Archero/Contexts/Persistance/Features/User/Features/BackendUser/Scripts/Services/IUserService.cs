using Cysharp.Threading.Tasks;
using Persistence.Features.Auth;
using Template.Scripts.DataHolders;

namespace Persistence.Features.User.Scripts
{
    public interface IUserService
    {
        public UniTask InitAsync();
        public UniTask SyncCurrentUserAsync();
        
        public UserData GetCachedUser(); 
        
        public UniTask<Result<UserData>> GetUser(string id, bool forceRefresh = false);
        public UniTask<Result> CreateUser(UserData user);
        public UniTask<Result> UpdateUser(UserData user);
        public UniTask<Result> DeleteUser(string id);
        public UniTask<Result> RegisterNewUserAsync(UserData userData);
        public void ClearUserData();
        UniTask<Result<bool>> UserExistsOnServer(string userId);
    }
}
