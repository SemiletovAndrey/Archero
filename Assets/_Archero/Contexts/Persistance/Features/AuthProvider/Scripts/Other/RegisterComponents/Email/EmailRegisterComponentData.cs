namespace Contexts.Persistence.Features.AuthProvider
{
    public class EmailRegisterComponentData: RegisterComponentData
    {
        public EmailRegisterComponentData(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; private set; }
        public string Password { get; private set; }
    }
}