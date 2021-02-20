using System;
using GameStreamSearch.Application.ValueObjects;

namespace GameStreamSearch.Application.Enums
{
    public static class StreamPlatformType
    {
        public static StreamPlatform YouTube = new StreamPlatform("youtube", "YouTube");
        public static StreamPlatform Twitch = new StreamPlatform("twitch", "Twitch");
        public static StreamPlatform DLive = new StreamPlatform("dlive", "DLive");
    }
}
