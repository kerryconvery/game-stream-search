using System;
using System.Collections.Generic;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Dto
{
    public class ChannelDto
    {
        public string ChannelName { get; init; }
        public StreamPlatformType StreamPlatform { get; init; }
        public string AvatarUrl { get; init; }
        public string ChannelUrl { get; init; }
    }

    public class ChannelListDto
    {
        public IEnumerable<ChannelDto> Items { get; init; }
    }
}
