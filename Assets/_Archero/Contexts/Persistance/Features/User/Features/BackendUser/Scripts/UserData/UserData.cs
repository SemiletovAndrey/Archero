using Template.Features.DataStorage;

namespace Persistence.Features.Auth
{
    public class UserData : ISaveData
    {
        public string FirebaseUid { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        
        public UserData CreateCopy()
        {
            return MemberwiseClone() as UserData;
        }
        
        public class Builder
        {
            private UserData _user;

            private Builder()
            {
            }

            public UserData Bake()
            {
                return _user;
            }

            public Builder WithEmail(string email)
            {
                _user.Email = email;

                return this;
            }

            public Builder WithUid(string uid)
            {
                _user.FirebaseUid = uid;

                return this;
            }

            public Builder WithUsername(string username)
            {
                _user.Username = username;

                return this;
            }
            
            public Builder WithAge(int age)
            {
                _user.Age = age;

                return this;
            }
            
            public static Builder FromEmpty(string id)
            {
                Builder instance = new Builder
                {
                    _user = new UserData
                    {
                        FirebaseUid = id,
                    },
                };

                return instance;
            }

            public static Builder FromCopy(UserData user)
            {
                Builder instance = new Builder
                {
                    _user = user.CreateCopy()
                };

                return instance;
            }
        }
    }
}