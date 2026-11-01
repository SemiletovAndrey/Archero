using System;
using System.Collections.Generic;

namespace Template.Scripts.DataHolders
{
    public class EventWithPriority : IEventWithPriority
    {
        private readonly List<(Action, int)> _callbacks = new();
        
        public void Subscribe(Action callback, int priority)
        {
            _callbacks.Add((callback, priority));
            SortCallbacks();
        }

        public void Unsubscribe(Action callback)
        {
            _callbacks.RemoveAll(item => item.Item1 == callback);
            SortCallbacks();
        }

        public void Invoke()
        {
            foreach (var callback in _callbacks)
            {
                callback.Item1();
            }
        }

        private void SortCallbacks()
        {
            _callbacks.Sort((item1, item2) => item1.Item2.CompareTo(item2.Item2));
        }
    }
    
    public class EventWithPriority<T> : IEventWithPriority<T>
    {
        private readonly List<(Action<T>, int)> _callbacks = new();
        
        public void Subscribe(Action<T> callback, int priority)
        {
            _callbacks.Add((callback, priority));
            SortCallbacks();
        }

        public void Unsubscribe(Action<T> callback)
        {
            _callbacks.RemoveAll(item => item.Item1 == callback);
            SortCallbacks();
        }

        public void Invoke(T payload)
        {
            foreach (var callback in _callbacks)
            {
                callback.Item1(payload);
            }
        }

        private void SortCallbacks()
        {
            _callbacks.Sort((item1, item2) => item1.Item2.CompareTo(item2.Item2));
        }
    }
}