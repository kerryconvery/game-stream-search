using System;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Dto
{
    public class PlatformChannelDto
    {
        public string ChannelName { get; init; }
        public string StreamPlatformId { get; init; }
        public string AvatarUrl { get; init; }
        public string ChannelUrl { get; init; }

        public Channel ToChannel(DateTime dateRegistered)
        {
            return new Channel(ChannelName, StreamPlatformId, dateRegistered, AvatarUrl, ChannelUrl);
        }
    }
}
