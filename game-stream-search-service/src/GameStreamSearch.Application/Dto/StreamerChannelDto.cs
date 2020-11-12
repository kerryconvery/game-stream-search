using System;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Dto
{
    public class StreamerChannelDto
    {
        public string ChannelName { get; set; }
        public StreamingPlatform Platform { get; set; }
        public string AvatarUrl { get; set; }
    }
}
