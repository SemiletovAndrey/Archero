using System;
using System.Collections.Generic;

namespace Contexts.Scripts.Utilities.Reactive
{
    public class ReactiveVariable<T> : IReadOnlyVariable<T>
    {
        private readonly List<Subscriber<T, T>> _subscribers = new();
        private readonly List<Subscriber<T, T>> _toAdd = new();
        private readonly List<Subscriber<T, T>> _toRemove = new();

        private T _value;
        private IEqualityComparer<T> _equalityComparer;

        public ReactiveVariable() : this(default)
        {
        }

        public ReactiveVariable(T value) : this(value, EqualityComparer<T>.Default)
        {
            _value = value;
        }

        public ReactiveVariable(T value, EqualityComparer<T> equalityComparer)
        {
            _value = value;
            _equalityComparer = equalityComparer;
        }

        public T Value
        {
            get => _value;
            set
            {
                T oldValue = _value;
                _value = value;

                if (_equalityComparer.Equals(oldValue, value) == false)
                    Invoke(oldValue, value);
            }
        }

        public IDisposable Subscribe(Action<T, T> action)
        {
            Subscriber<T, T> subscriber = new Subscriber<T, T>(action, RemoveSubscriber);
            _toAdd.Add(subscriber);
            return subscriber;
        }

        private void RemoveSubscriber(Subscriber<T, T> subscriber)
        {
            _toRemove.Add(subscriber);
        }

        private void Invoke(T oldValue, T newValue)
        {
            if (_toAdd.Count > 0)
            {
                _subscribers.AddRange(_toAdd);
                _toAdd.Clear();
            }

            if (_toRemove.Count > 0)
            {
                foreach (Subscriber<T, T> subscriber in _toRemove)
                    _subscribers.Remove(subscriber);

                _toRemove.Clear();
            }

            foreach (Subscriber<T, T> subscriber in _subscribers)
            {
                subscriber.Invoke(oldValue, newValue);
            }
        }
    }
}