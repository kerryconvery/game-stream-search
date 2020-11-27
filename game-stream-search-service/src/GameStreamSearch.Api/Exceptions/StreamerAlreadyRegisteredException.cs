using System;
namespace GameStreamSearch.Application.Exceptions
{
    public class StreamerAlreadyRegisteredException : Exception
    {
        public StreamerAlreadyRegisteredException(string streamerName, string platformName)
            : base($"Channel {streamerName} is already registered for {platformName}")
        {
        }
    }
}
