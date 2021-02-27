using System;
using System.Collections.Generic;

namespace GameStreamSearch.StreamProviders.YouTube.Dto.V3
{
    public class YouTubeChannelsDto
    {
        public IEnumerable<YouTubeChannelDto> items { get; set; }
    }
}
