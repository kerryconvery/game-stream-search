using System;
using System.Collections.Generic;

namespace GameStreamSearch.Gateways.Dto.YouTube.YouTubeV3
{
    public class YouTubeChannelsDto
    {
        public IEnumerable<YouTubeChannelDto> items { get; set; }
    }
}
