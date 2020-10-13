﻿using System;
using System.Collections.Generic;

namespace GameStreamSearch.StreamProviders.ProviderApi.YouTube.Dto.YouTubeV3
{
    public class YouTubeVideoSearchSnippetThumbnailDto
    {
        public string url { get; set; }
    }

    public class YouTubeVideoSearchSnippetThumbnailsDto
    {
        public YouTubeVideoSearchSnippetThumbnailDto medium { get; set; }
        public YouTubeVideoSearchSnippetThumbnailDto high { get; set; }
    }

    public class YouTubeVideoSearchSnippetDto
    {
        public string channelId { get; set; }
        public string title { get; set; }
        public YouTubeVideoSearchSnippetThumbnailsDto thumbnails { get; set; }
        public string channelTitle { get; set; }

    }

    public class YouTubeVideoSearchItemIdDto
    {
        public string videoId { get; set; }
    }

    public class YouTubeVideoSearchItemDto
    {
        public YouTubeVideoSearchItemIdDto id { get; set; }
        public YouTubeVideoSearchSnippetDto snippet { get; set; }

    }

    public class YouTubeVideoSearchDto
    {
        public IEnumerable<YouTubeVideoSearchItemDto> items { get; set; }
        public string nextPageToken { get; set; }
    }
}