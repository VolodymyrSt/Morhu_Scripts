using System;
using System.Collections.Generic;

namespace Morhu.Util
{
    public class R3Variable<T> : IReadOnlyR3Variable<T>
    {
        public event Action<T, T> Changed;

        private T _value;
        private EqualityComparer<T> _comparer;

        public R3Variable() : this(default(T)) { }
        public R3Variable(T value) : this(value, EqualityComparer<T>.Default) { }
        public R3Variable(T value, EqualityComparer<T> comparer)
        {
            _value = value;
            _comparer = comparer;
        }

        public T Value
        {
            get => _value;
            set
            {
                var oldValue = _value;
                _value = value;

                if (!_comparer.Equals(oldValue, value))
                    Changed?.Invoke(oldValue, _value);
            }
        }
    }
}

