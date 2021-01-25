using System;

namespace GameStreamSearch.Types
{
    public struct MaybeResult<V, E> where E : Enum
    {
        public static MaybeResult<V, E> Success(V value)
        {
            return new MaybeResult<V, E>
            {
                IsSuccess = true,
                Value = Maybe<V>.ToMaybe(value)
            };
        }

        public static MaybeResult<V, E> Success(Maybe<V> value)
        {
            return new MaybeResult<V, E>
            {
                IsSuccess = true,
                Value = value,
            };
        }

        public static MaybeResult<V, E> Fail(E error)
        {
            return new MaybeResult<V, E>
            {
                Value = Maybe<V>.Nothing,
                IsSuccess = false,
                Error = error,
            };
        }

        public bool IsSuccess { get; init; }
        public bool IsFailure => !IsSuccess;
        public E Error { get; init; }
        public Maybe<V> Value { get; init; }
    }
}
