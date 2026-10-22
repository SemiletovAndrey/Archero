using Newtonsoft.Json.Linq;

namespace Template.Features.Backend.Socket
{
    public abstract class RegisteredSocketEvent
    {
        public abstract string EventName { get; }
        
        public abstract bool CheckEvent(string eventName);
        public abstract void InvokeCallback(JToken jsonToken);
    }
}