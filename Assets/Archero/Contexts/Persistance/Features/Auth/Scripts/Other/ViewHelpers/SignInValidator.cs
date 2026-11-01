using System.Net.Mail;
using Template.Utilities.Primitives;

namespace Persistance.Features.Auth
{
    public static class SignInValidator
    {
        private const int MinAge = 13;
        private const int MaxAge = 120;

        public static bool ValidateEmail(string email, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = "Enter email";
                return false;
            }

            try
            {
                MailAddress addr = new MailAddress(email);
                if (addr.Address == email)
                {
                    errorMessage = null;
                    return true;
                }
            }
            catch
            {
                /* fall through */
            }

            errorMessage = "Incorrect email";
            return false;
        }

        public static bool ValidatePassword(string password, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Enter password";
                return false;
            }

            if (password.Length < Password.MinLength)
            {
                errorMessage = $"Password must contain at least {Password.MinLength} characters";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool ValidateConfirmPassword(string password, string confirmPassword, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                errorMessage = "Confirm password";
                return false;
            }

            if (password != confirmPassword)
            {
                errorMessage = "Differently password";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool ValidateAge(string ageText, out int age, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(ageText) || !int.TryParse(ageText, out age))
            {
                errorMessage = "Enter a valid age";
                age = 0;
                return false;
            }

            if (age < MinAge || age > MaxAge)
            {
                errorMessage = "Incorrect age";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}