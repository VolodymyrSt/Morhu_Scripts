using System;
using System.Collections.Generic;

namespace Morhu.Util
{
    public class R3List<T>
    {
        public event Action<T> Added;
        public event Action<T> Removed;

        private readonly List<T> _list;
        public IReadOnlyList<T> Values => _list;

        public R3List() => _list = new List<T>();

        public virtual void Add(T element)
        {
            _list.Add(element);
            Added?.Invoke(element);
        }
        
        public virtual void Remove(T element)
        {
            _list.Remove(element);
            Removed?.Invoke(element);
        }
    }
}

