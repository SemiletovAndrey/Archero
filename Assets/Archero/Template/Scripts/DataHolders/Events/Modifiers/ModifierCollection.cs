using System;
using System.Collections.Generic;

namespace Template.Scripts.DataHolders
{
    public class ModifierCollection<T> : IModifierCollection<T>
    {
        private List<(Func<T, T>, int)> _modifiers = new();
        
        public void Add(Func<T, T> modifier, int priority)
        {
            _modifiers.Add((modifier, priority));
            SortModifiers();
        }

        public void Remove(Func<T, T> modifier)
        {
            _modifiers.RemoveAll(item => item.Item1 == modifier);
        }

        public T ApplyTo(T modifierModel)
        {
            for (int i = 0; i < _modifiers.Count; i++)
            {
                (Func<T, T>, int) callback = _modifiers[i];
                modifierModel = callback.Item1(modifierModel);
            }

            return modifierModel;
        }

        private void SortModifiers()
        {
            _modifiers.Sort((item1, item2) => item1.Item2.CompareTo(item2.Item2));
        }
    }
}