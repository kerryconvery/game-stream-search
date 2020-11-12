using System;
namespace GameStreamSearch.Application.Enums
{
    public enum StreamingPlatform
    {
        twitch,
        dlive,
        youtube,
    }

    public static class TypeExtensions
    {
        public static string GetFriendlyName(this StreamingPlatform streamingPlatform)
        {
            switch(streamingPlatform)
            {
                case StreamingPlatform.twitch: return "Twitch";
                case StreamingPlatform.youtube: return "YouTube";
                case StreamingPlatform.dlive: return "DLive";
                default: return streamingPlatform.ToString();
            }
        }
    }
}
