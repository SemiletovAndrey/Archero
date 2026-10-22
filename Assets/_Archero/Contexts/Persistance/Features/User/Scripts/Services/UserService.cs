using Contexts.Persistence.Features.AuthProvider;
using Cysharp.Threading.Tasks;
using Persistence.Features.Auth;
using Template.Features.DataStorage;
using Template.Scripts.DataHolders;
using UnityEngine;

namespace Persistence.Features.User.Scripts
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthProvider _authProvider;
        private readonly IDataStorageService _storageService;
        
        private UserData _userData;

        public UserService(IUserRepository userRepository, IAuthProvider authProvider,
            IDataStorageService storageService)
        {
            _userRepository = userRepository;
            _authProvider = authProvider;
            _storageService = storageService;
        }

        public async UniTask InitAsync()
        {
            if (await _authProvider.WaitForUserAsync())
            {
                await SyncCurrentUserAsync();
            }
        }

        public UniTask SyncCurrentUserAsync()
        {
            return UpdateUserOnServer();
        }

        public UserData GetCachedUser()
        {
            return _userData;
        } 
        
        public async UniTask<Result> RegisterNewUserAsync(UserData userData)
        {
            var result = await CreateUser(userData);
    
            if (result.IsFailure)
                return result;

            _userData = userData;
            _storageService.Save(_userData);

            return Result.Success();
        }

        public async UniTask<Result<UserData>> GetUser(string id, bool forceRefresh = false)
        {
            bool isCurrentUser = _authProvider.TryGetUser(out string currentUserId) && currentUserId == id;
        
            if (!forceRefresh && isCurrentUser && _userData != null)
            {
                return Result.Success(_userData);
            }

            Result<UserDto> result = await _userRepository.GetUser(id);
            if (result.IsFailure)
            {
                return Result.Failure<UserData>(result.Error);
            }

            UserData userData = ToUserData(result.Value);

            if (isCurrentUser)
            {
                _userData = userData;
                _storageService.Save(_userData);
            }

            return Result.Success(userData);
        }

        public UniTask<Result> CreateUser(UserData user)
        {
            var userDto = new UserDto
            {
                FirebaseUid = user.FirebaseUid,
                Email = user.Email,
                Age = user.Age
            };
            return _userRepository.CreateUser(userDto);
        }

        public UniTask<Result> UpdateUser(UserData user)
        {
            return _userRepository.UpdateUser(user);
        }

        public UniTask<Result> DeleteUser(string id)
        {
            return _userRepository.DeleteUser(id);
        }

        public void ClearUserData()
        {
            _userData = new UserData();
            _storageService.Save(_userData);
        }
        
        public UniTask<Result<bool>> UserExistsOnServer(string userId)
        {
            return _userRepository.UserExists(userId);
        }

        private static UserData ToUserData(UserDto dto)
        {
            return new UserData
            {
                FirebaseUid = dto.id.ToString(),
                Username = "",
                Email = dto.Email,
                Age = dto.Age
            };
        }
        
        private async UniTask UpdateUserOnServer()
        {
            if (!_authProvider.TryGetUser(out string userId))
            {
                return;
            }

            if (!_storageService.TryLoad(out _userData))
            {
                _userData = UserData.Builder.FromEmpty(userId).Bake();
            }
            else if (_userData.FirebaseUid == "")
            {
                _userData = UserData.Builder.FromEmpty(userId).Bake();
            }

            Result<bool> userExistsResult = await _userRepository.UserExists(userId);
            if (userExistsResult.IsFailure)
            {
                return;
            }

            if (userExistsResult.Value == false)
            {
                await CreateUser(_userData);
                
                return;
            }

            Result<UserData> userResult = await GetUser(userId, true);

            if (userResult.IsFailure)
            {
                return;
            }

            _userData = userResult.Value;
            _storageService.Save(_userData);
        }
    }
}
