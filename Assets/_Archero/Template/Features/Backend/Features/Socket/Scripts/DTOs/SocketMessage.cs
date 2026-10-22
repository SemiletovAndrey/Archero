using Newtonsoft.Json;

namespace Template.Features.Backend.Socket
{
    public class SocketMessage
    {
        [JsonProperty("op_code")] public string EventName { get; init; }
        [JsonProperty("op_args")] public object EventArgs { get; init; }
    }
}