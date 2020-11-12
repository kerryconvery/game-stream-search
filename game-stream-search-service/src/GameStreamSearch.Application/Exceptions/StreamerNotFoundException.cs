using System;
namespace GameStreamSearch.Application.Exceptions
{
    public class StreamerNotFoundException : Exception
    {
        public StreamerNotFoundException(string streamerId) : base($"Streamer for id {streamerId} is not registered")
        {
        }
    }
}
