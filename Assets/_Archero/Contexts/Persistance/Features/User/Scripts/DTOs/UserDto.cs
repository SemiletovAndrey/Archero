using Newtonsoft.Json;

namespace Persistence.Features.User.Scripts
{
    public class UserDto
    {
        [JsonProperty(PropertyName = "id")]
        public long? id { get; set; }
        
        [JsonProperty(PropertyName = "FirebaseUid")]
        public string FirebaseUid { get; set; }

        /*[JsonProperty(PropertyName = "username")]
        public string Username { get; set; }*/
        
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
        
        [JsonProperty(PropertyName = "age")]
        public int Age { get; set; }
    }
}