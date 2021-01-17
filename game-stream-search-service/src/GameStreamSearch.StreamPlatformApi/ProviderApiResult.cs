namespace GameStreamSearch.StreamPlatformApi
{
    public enum ProviderApiOutcome
    {
        Success,
        ProviderNotAvailable,
    }

    public class ProviderApiResult<T>
    {
        public static ProviderApiResult<T> Success(T value)
        {
            return new ProviderApiResult<T> { Value = value, Outcome = ProviderApiOutcome.Success };
        }

        public static ProviderApiResult<T> ProviderNotAvilable()
        {
            return new ProviderApiResult<T> { Outcome = ProviderApiOutcome.ProviderNotAvailable };
        }

        public T Value { get; init; }
        public ProviderApiOutcome Outcome { get; init; }
    }
}
