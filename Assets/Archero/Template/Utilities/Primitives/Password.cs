using System.Collections.Generic;
using Template.Scripts.DataHolders;

namespace Template.Utilities.Primitives
{
    public class Password : ValueObject
    {
    public const int MinLength = 6;

    private Password(string value) 
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Password> Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return Result.Failure<Password>(new Error(
                "Password.Empty",
                "Password is empty."));
        }

        if (password.Length < MinLength)
        {
            return Result.Failure<Password>(new Error(
                "Password.TooShort",
                $"Password must be greater than {MinLength} symbols."));
        }
            
        return new Password(password);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    public static implicit operator string(Password password) =>
        password.Value;
    }
}