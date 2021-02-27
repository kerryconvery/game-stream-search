
using System.Collections.Generic;

namespace GameStreamSearch.StreamProviders.Dto.YouTube.YouTubeV3
{
    public class YouTubeVideoLiveStreamingDetailsDto
    {
        public int concurrentViewers { get; set; }
    }

    public class YouTubeVideoDto
    {
        public string id { get; set; }
        public YouTubeVideoLiveStreamingDetailsDto liveStreamingDetails { get; set; }
    }

    public class YouTubeVideosDto
    {
        public IEnumerable<YouTubeVideoDto> items { get; set; }
    }
}
