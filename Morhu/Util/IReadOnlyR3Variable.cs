using System;

namespace Morhu.Util
{
    public interface IReadOnlyR3Variable<T>
    {
        event Action<T, T> Changed;
        T Value { get; set; }
    }
}