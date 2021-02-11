using System;
using System.Threading.Tasks;

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

        public MaybeResult<TResult, E> Select<TResult>(Func<V, TResult> selector)
        {
            if (IsFailure)
            {
                return MaybeResult<TResult, E>.Fail(Error);
            }

            return MaybeResult<TResult, E>.Success(Value.Select(selector));
        }

        public async Task<MaybeResult<TResult, E>> SelectAsync<TResult>(Func<V, Task<TResult>> selector)
        {
            if (IsFailure)
            {
                return MaybeResult<TResult, E>.Fail(Error);
            }

            var result = await Value.SelectAsync(selector);

            return MaybeResult<TResult, E>.Success(result);
        }

        public MaybeResult<TResult, E> Chain<TResult>(Func<V, MaybeResult<TResult, E>> selector)
        {
            if (IsFailure)
            {
                return MaybeResult<TResult, E>.Fail(Error);
            }

            return MaybeResult<TResult, E>.Success(selector(Value.value).Value);
        }

        public async Task<MaybeResult<TResult, E>> ChainAsync<TResult>(Func<V, Task<MaybeResult<TResult, E>>> selector)
        {
            if (IsFailure)
            {
                return MaybeResult<TResult, E>.Fail(Error);
            }

            var result = await selector(Value.value);

            return MaybeResult<TResult, E>.Success(result.Value);
        }


        public V GetOrElse(V orElse)
        {
            if (IsFailure)
            {
                return orElse;
            }

            return Value.GetOrElse(orElse);
        }
    }
}
