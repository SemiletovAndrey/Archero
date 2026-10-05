using System;

namespace Template.Scripts.DataHolders
{
    public interface IModifierCollection<T>
    {
        public void Add(Func<T, T> modifier, int priority = 0);
        public void Remove(Func<T, T> modifier);
    }
}