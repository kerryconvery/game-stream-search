using System;
namespace GameStreamSearch.Application.Exceptions
{
    public class StreamerNotFoundOnPlatformException : Exception
    {
        public StreamerNotFoundOnPlatformException(string streamerName, string platformName)
            : base($"Streamer {streamerName} was not found on {platformName}")
        {
        }
    }
}
