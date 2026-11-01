using System;
using System.Collections;
using System.Collections.Generic;

namespace Template.Scripts.DataHolders
{
    public class TypesList<TList, TData> : IEnumerable<Type> where TList : TypesList<TList, TData>
    {
        private readonly List<Type> _items = new();

        public TList Add<TItem>() where TItem : TData
        {
            _items.Add(typeof(TItem));
            return (TList)this;
        }

        public IEnumerator<Type> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}