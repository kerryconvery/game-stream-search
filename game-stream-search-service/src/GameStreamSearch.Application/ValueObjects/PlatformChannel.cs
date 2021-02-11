using System;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.ValueObjects
{
    public class PlatformChannel
    {
        public string ChannelName { get; init; }
        public StreamPlatformType Platform { get; init; }
        public string AvatarUrl { get; init; }
        public string ChannelUrl { get; init; }

        public Channel ToChannel(DateTime dateRegistered)
        {
            return new Channel(ChannelName, Platform, dateRegistered, AvatarUrl, ChannelUrl);
        }
    }
}
