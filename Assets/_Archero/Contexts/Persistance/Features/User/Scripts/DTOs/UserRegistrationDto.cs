using System;
using Newtonsoft.Json;

namespace Persistence.Features.User.Scripts
{
    [Serializable]
    public class UserRegistrationDto
    {
        [JsonProperty(PropertyName = "age")]
        public int Age { get; set; }
    }
}