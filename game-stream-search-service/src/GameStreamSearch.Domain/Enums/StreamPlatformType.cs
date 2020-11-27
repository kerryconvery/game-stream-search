using System;
namespace GameStreamSearch.Domain.Enums
{
    public enum StreamPlatformType
    {
        twitch,
        dlive,
        youtube,
    }

    public static class TypeExtensions
    {
        public static string GetFriendlyName(this StreamPlatformType streamingPlatform)
        {
            switch(streamingPlatform)
            {
                case StreamPlatformType.twitch: return "Twitch";
                case StreamPlatformType.youtube: return "YouTube";
                case StreamPlatformType.dlive: return "DLive";
                default: return streamingPlatform.ToString();
            }
        }
    }
}
