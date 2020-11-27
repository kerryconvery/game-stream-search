using System;
namespace GameStreamSearch.Domain.Exceptions
{
    public class StreamerNotFoundOnPlatformException : Exception
    {
        public StreamerNotFoundOnPlatformException(string streamerName, string platformName)
            : base($"Channel {streamerName} was not found on {platformName}")
        {
        }
    }
}
