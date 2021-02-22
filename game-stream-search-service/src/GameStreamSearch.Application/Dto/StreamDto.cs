namespace GameStreamSearch.Application.Types
{
    public class StreamDto
    {
        public string StreamTitle { get; init; }
        public string StreamThumbnailUrl { get; init; }
        public string StreamUrl { get; init; }
        public string StreamerName { get; init; }
        public string StreamerAvatarUrl { get; init; }
        public string PlatformName { get; set; }
        public bool IsLive { get; init; }
        public int Views { get; init; }
    }
}
