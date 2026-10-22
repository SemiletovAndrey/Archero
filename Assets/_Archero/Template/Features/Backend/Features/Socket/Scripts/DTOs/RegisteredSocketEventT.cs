using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Template.Features.Backend.Socket
{
    public class RegisteredSocketEvent<T> : RegisteredSocketEvent
    {
        private string _eventName;
        private Action<T> _callback;

        public RegisteredSocketEvent(string eventName, Action<T> callback)
        {
            _eventName = eventName;
            _callback = callback;
        }

        public override string EventName => _eventName;

        public override bool CheckEvent(string eventName)
        {
            return _eventName == eventName;
        }

        public override void InvokeCallback(JToken jsonToken)
        {
            T args = jsonToken.ToObject<T>();
            
            Debug.Log($"ReceivedMessage {EventName}\nArgs: {jsonToken}");
            
            _callback.Invoke(args);
        }
    }
}