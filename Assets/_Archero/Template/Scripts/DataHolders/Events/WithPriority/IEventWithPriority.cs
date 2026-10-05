using System;

namespace Template.Scripts.DataHolders
{
    public interface IEventWithPriority
    {
        public void Subscribe(Action callback, int priority = 0);
        public void Unsubscribe(Action callback);
    }
    
    public interface IEventWithPriority<out T>
    {
        public void Subscribe(Action<T> callback, int priority = 0);
        public void Unsubscribe(Action<T> callback);
    }
}