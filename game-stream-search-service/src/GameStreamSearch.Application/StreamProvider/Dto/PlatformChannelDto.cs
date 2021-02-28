using System;
using GameStreamSearch.Domain.Channel;

namespace GameStreamSearch.Application.StreamProvider.Dto
{
    public class PlatformChannelDto
    {
        public string ChannelName { get; init; }
        public string StreamPlatformName { get; init; }
        public string AvatarUrl { get; init; }
        public string ChannelUrl { get; init; }

        public Channel ToChannel(DateTime dateRegistered)
        {
            return new Channel(ChannelName, StreamPlatformName, dateRegistered, AvatarUrl, ChannelUrl);
        }
    }
}
