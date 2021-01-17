using System;

namespace GameStreamSearch.Result
{
    public class BaseResult<V, O>
    {
        public O Outcome { get; init; }
        public V? Value { get; init; }
    }
}
