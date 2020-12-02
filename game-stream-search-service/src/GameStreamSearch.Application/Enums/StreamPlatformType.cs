using System;
namespace GameStreamSearch.Application.Enums
{
    public enum StreamPlatformType
    {
        Twitch = 0,
        DLive = 1,
        YouTube = 2,
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
