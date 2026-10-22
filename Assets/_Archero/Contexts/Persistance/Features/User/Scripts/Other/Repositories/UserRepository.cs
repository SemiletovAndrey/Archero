using System.Collections.Concurrent;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Persistence.Features.Auth;
using Template.Features.Backend.Http;
using Template.Scripts.DataHolders;
using UnityEngine;

namespace Persistence.Features.User.Scripts
{
    public class UserRepository : BaseHttpHandler, IUserRepository
    {
        protected override string Route => "user";
        protected override bool CheckInternet => true;
        
        public UniTask<Result<UserDto>> GetUser(string id)
        {
            return CreateGetRequest<UserDto>("me").Send();
        }

        public UniTask<Result<bool>> UserExists(string id)
        {
            return CreateGetRequest<bool>("me","exist").Send();
        }

        public async UniTask<Result> CreateUser(UserDto user)
        {
            UserRegistrationDto userRegistrationDto = new UserRegistrationDto
            {
                Age = user.Age,
            };

            Result<UserDto> result = await CreatePostRequest<UserDto>("register")
                .WithHeaders(new ConcurrentDictionary<string, string>()
                {
                    ["firebaseUid"] = user.FirebaseUid,
                    ["email"] = user.Email,
                    ["Content-Type"] = "application/json",
                })
                .WithBody(userRegistrationDto)
                .Send();
            
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            return result;
        }

        public async UniTask<Result> DeleteUser(string id)
        {
            Result result = await CreateDeleteRequest("firebase_id", id).Send();
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
            
            return result;
        }

        public async UniTask<Result> UpdateUser(UserData user)
        {
            var result = await CreatePutRequest<UserData>("update", user).Send();
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
            
            return result;
        }
    }
}
