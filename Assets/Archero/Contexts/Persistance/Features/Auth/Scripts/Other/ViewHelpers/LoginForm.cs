namespace Persistance.Features.Auth
{
    public readonly struct LoginForm
    {
        public readonly string Email;
        public readonly string Password;

        public LoginForm(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}