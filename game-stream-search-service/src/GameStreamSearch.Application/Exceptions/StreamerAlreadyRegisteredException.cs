using System;
namespace GameStreamSearch.Application.Exceptions
{
    public class StreamerAlreadyRegisteredException : Exception
    {
        public StreamerAlreadyRegisteredException(string streamerName, string platformName)
            : base($"Streamer {streamerName} is already registered for {platformName}")
        {
        }
    }
}
