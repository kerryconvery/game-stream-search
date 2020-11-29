using System;
namespace GameStreamSearch.Application.Enums
{
    public enum StreamPlatformType
    {
        Twitch,
        DLive,
        YouTube,
    }

    public static class TypeExtensions
    {
        public static string GetFriendlyName(this StreamPlatformType streamingPlatform)
        {
            switch(streamingPlatform)
            {
                case StreamPlatformType.Twitch: return "Twitch";
                case StreamPlatformType.YouTube: return "YouTube";
                case StreamPlatformType.DLive: return "DLive";
                default: return streamingPlatform.ToString();
            }
        }
    }
}
