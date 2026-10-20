using System;
using System.Collections.Generic;

namespace Template.Features.NewInputFeature
{
    public class InputBus : IInputBus
    {
        private readonly Dictionary<Type, object> _subscribers = new();

        public void Subscribe<TSignal>(Action<TSignal> callback) where TSignal : struct, IInputSignal
        {
            Type type = typeof(TSignal);
            if (!_subscribers.TryGetValue(type, out var list))
            {
                list = new List<Action<TSignal>>();
                _subscribers[type] = list;
            }
            ((List<Action<TSignal>>)list).Add(callback);
        }

        public void Publish<TSignal>(TSignal signal) where TSignal : struct, IInputSignal
        {
            Type type = typeof(TSignal);
            if (_subscribers.TryGetValue(type, out var list))
            {
                var callbacks = (List<Action<TSignal>>)list;
                for (int i = 0; i < callbacks.Count; i++)
                {
                    callbacks[i]?.Invoke(signal);
                }
            }
        }

        public void Unsubscribe<TSignal>(Action<TSignal> callback) where TSignal : struct, IInputSignal
        {
            Type type = typeof(TSignal);
            if (_subscribers.TryGetValue(type, out var list))
            {
                ((List<Action<TSignal>>)list).Remove(callback);
            }
        }
    }
}