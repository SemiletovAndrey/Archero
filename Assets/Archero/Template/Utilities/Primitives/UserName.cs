using System.Collections.Generic;
using Template.Scripts.DataHolders;

namespace Template.Utilities.Primitives
{
    public class UserName : ValueObject
    {
        private UserName(string value) 
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<UserName> Create(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return Result.Failure<UserName>(new Error(
                    "UserName.Empty",
                    "User name is empty."));
            }

            return new UserName(userName);
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public static implicit operator string(UserName password) =>
            password.Value;
    }
}