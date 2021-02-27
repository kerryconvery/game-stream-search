using System;
using System.Collections.Generic;

namespace GameStreamSearch.StreamProviders.YouTube.Gateways.Dto.V3
{
    public class YouTubeChannelsDto
    {
        public IEnumerable<YouTubeChannelDto> items { get; set; }
    }
}
