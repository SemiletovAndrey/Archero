using System;

namespace Contexts.Scripts.Utilities.Reactive
{
    public interface IReadOnlyVariable<T>
    {
        public T Value { get; }
        
        public IDisposable Subscribe(Action<T, T> action);
    }
}