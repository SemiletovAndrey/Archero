using Contexts.Persistence.Features.Auth;

namespace Contexts.Persistence.Features.AuthProvider
{
    public class RegistrationUserModel
    {
        public RegistrationUserModel(string id, string email, RegistrationType registrationType)
        {
            Id = id;
            Email = email;
            
            RegistrationType = registrationType;
        }
        
        public RegistrationUserModel(string id, string email, string name, RegistrationType registrationType)
        {
            Id = id;
            Email = email;
            
            Name = name;
            RegistrationType = registrationType;
        }
        
        public string Id { get; private set; }
        public string Email { get; private set; }
        
        public string Name { get; private set; }
        public RegistrationType RegistrationType { get; private set; }
    }
}