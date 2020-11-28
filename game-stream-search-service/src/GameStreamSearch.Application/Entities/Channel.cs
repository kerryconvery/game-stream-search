using System;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Entities
{
    public class Channel
    {
        public string ChannelName { get; set; }
        public StreamPlatformType StreamPlatform { get; set; }
        public DateTime DateRegistered { get; set; }
        public string AvatarUrl { get; set; }
        public string ChannelUrl { get; set; }
    }
}
