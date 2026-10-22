using Cysharp.Threading.Tasks;
using Persistence.Features.Auth;
using Template.Scripts.DataHolders;

namespace Persistence.Features.User.Scripts
{
    public interface IUserRepository
    {
        public UniTask<Result<UserDto>> GetUser(string id);
        public UniTask<Result<bool>> UserExists(string id);

        public UniTask<Result> CreateUser(UserDto user);
        public UniTask<Result> DeleteUser(string id);
        public UniTask<Result> UpdateUser(UserData user);
    }
}
