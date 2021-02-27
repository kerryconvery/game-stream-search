namespace GameStreamSearch.Application.StreamProvider.Dto
{
    public class PlatformStreamDto
    {
        public string StreamTitle { get; init; }
        public string StreamThumbnailUrl { get; init; }
        public string StreamUrl { get; init; }
        public string StreamerName { get; init; }
        public string StreamerAvatarUrl { get; init; }
        public bool IsLive { get; init; }
        public int Views { get; init; }
    }
}
