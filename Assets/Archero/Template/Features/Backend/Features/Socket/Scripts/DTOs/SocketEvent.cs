using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Features.Backend.Socket
{
    public class SocketEvent
    {
        [JsonProperty("event")] public string EventName { get; private set; }
        [JsonProperty("event_args")] public JToken EventArgs { get; private set; }
    }
}