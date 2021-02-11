using System;
using GameStreamSearch.Application.Entities;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.ValueObjects
{
    public class ChannelDto
    {
        public string ChannelName { get; init; }
        public StreamPlatformType StreamPlatform { get; init; }
        public string StreamPlatformDisplayName { get; init; }
        public string AvatarUrl { get; init; }
        public string ChannelUrl { get; init; }

        public static ChannelDto FromEntity(Channel channel)
        {
            return new ChannelDto
            {
                ChannelName = channel.ChannelName,
                StreamPlatform = channel.StreamPlatform,
                StreamPlatformDisplayName = channel.StreamPlatform.GetFriendlyName(),
                AvatarUrl = channel.AvatarUrl,
                ChannelUrl = channel.ChannelUrl,
            };
        }
    }
}
