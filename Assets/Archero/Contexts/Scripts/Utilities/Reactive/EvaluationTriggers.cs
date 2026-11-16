using System;
using System.Collections.Generic;

namespace Contexts.Scripts.Utilities.Reactive
{
    public abstract class EvaluationTriggers
    {
        private readonly List<Func<Action, IDisposable>> _sources = new(2);

        public EvaluationTriggers On<T>(ReactiveVariable<T> variable) where T : IEquatable<T>
        {
            _sources.Add(onChanged => variable.Subscribe((_, __) => onChanged()));
            return this;
        }

        public IDisposable Subscribe(Action onChanged)
        {
            var subscriptions = new Subscriptions(_sources.Count);

            for (int i = 0; i < _sources.Count; i++)
                subscriptions.Add(_sources[i](onChanged));

            return subscriptions;
        }

        private sealed class Subscriptions : IDisposable
        {
            private readonly List<IDisposable> _items;

            public Subscriptions(int capacity) => _items = new List<IDisposable>(capacity);

            public void Add(IDisposable item) => _items.Add(item);

            public void Dispose()
            {
                for (int i = 0; i < _items.Count; i++)
                    _items[i]?.Dispose();

                _items.Clear();
            }
        }
    }
}