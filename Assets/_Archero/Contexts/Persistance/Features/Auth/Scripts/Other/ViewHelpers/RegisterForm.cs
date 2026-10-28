namespace Persistance.Features.Auth
{
    public readonly struct RegisterForm
    {
        public readonly string Email;
        public readonly string Password;
        public readonly string ConfirmPassword;
        public readonly string Age;

        public RegisterForm(string email, string password, string confirmPassword, string age)
        {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
            Age = age;
        }
    }
}