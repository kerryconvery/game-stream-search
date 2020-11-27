using System;
using GameStreamSearch.Domain.Enums;

namespace GameStreamSearch.Application.Dto
{
    public class PlatformChannelDto
    {
        public string ChannelName { get; init; }
        public StreamPlatformType Platform { get; init; }
        public string AvatarUrl { get; init; }
    }
}
