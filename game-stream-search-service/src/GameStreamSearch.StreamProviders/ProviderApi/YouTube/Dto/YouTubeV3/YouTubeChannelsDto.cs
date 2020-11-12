using System;
using System.Collections.Generic;

namespace GameStreamSearch.StreamProviders.ProviderApi.YouTube.Dto.YouTubeV3
{
    public class YouTubeChannelSnippetThumbnailDto
    {
        public string url { get; set; }
    }

    public class YouTubeChannelSnippetThumbnailsDto
    {
        public YouTubeChannelSnippetThumbnailDto @default { get; set; }
    }

    public class YouTubeChannelSnippetDto
    {
        public string title { get; set; }
        public YouTubeChannelSnippetThumbnailsDto thumbnails { get; set; }
    }

    public class YouTubeChannelDto
    {
        public string id { get; set; }
        public YouTubeChannelSnippetDto snippet { get; set; }
    }

    public class YouTubeChannelsDto
    {
        public IEnumerable<YouTubeChannelDto> items { get; set; }
    }
}
