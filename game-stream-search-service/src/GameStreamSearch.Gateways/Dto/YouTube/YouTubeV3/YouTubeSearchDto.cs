using System;
using System.Collections.Generic;

namespace GameStreamSearch.Gateways.Dto.YouTube.YouTubeV3
{
    public class YouTubeSearchSnippetThumbnailDto
    {
        public string url { get; set; }
    }

    public class YouTubeSearchSnippetThumbnailsDto
    {
        public YouTubeSearchSnippetThumbnailDto medium { get; set; }
    }

    public class YouTubeSearchSnippetDto
    {
        public string channelId { get; set; }
        public string title { get; set; }
        public YouTubeSearchSnippetThumbnailsDto thumbnails { get; set; }
        public string channelTitle { get; set; }

    }

    public class YouTubeSearchItemIdDto
    {
        public string videoId { get; set; }
    }

    public class YouTubeSearchItemDto
    {
        public YouTubeSearchItemIdDto id { get; set; }
        public YouTubeSearchSnippetDto snippet { get; set; }

    }

    public class YouTubeSearchDto
    {
        public IEnumerable<YouTubeSearchItemDto> items { get; set; }
        public string nextPageToken { get; set; }
    }
}
