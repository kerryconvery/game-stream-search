using System;

namespace GameStreamSearch.Types
{
    public struct Result<E>
    {
        private E Error;
        private bool IsSuccess;

        public static Result<E> Success()
        {
            return new Result<E>()
            {
                IsSuccess = true,
            };
        }

        public static Result<E> Fail(E error)
        {
            return new Result<E>
            {
                IsSuccess = false,
                Error = error,
            };
        }

        public TResult Check<TResult>(Func<TResult> onSuccess, Func<E, TResult> onFailure)
        {
            if(IsSuccess)
            {
                return onSuccess();
            }

            return onFailure(Error);
        }
    }
}
