using System;
namespace GameStreamSearch.Domain.Exceptions
{
    public class StreamerNotFoundException : Exception
    {
        public StreamerNotFoundException(string streamerId) : base($"Channel for id {streamerId} is not registered")
        {
        }
    }
}
