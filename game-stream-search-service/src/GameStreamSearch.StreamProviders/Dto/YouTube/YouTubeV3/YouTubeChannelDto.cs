namespace GameStreamSearch.StreamProviders.Dto.YouTube.YouTubeV3
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
}
