using System;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Dto
{
    public class StreamDto
    {
        public string StreamTitle { get; init; }
        public string StreamThumbnailUrl { get; init; }
        public string StreamUrl { get; init; }
        public string StreamerName { get; init; }
        public string StreamerAvatarUrl { get; init; }
        public string PlatformId { get; set; }
        public bool IsLive { get; init; }
        public int Views { get; init; }
    }
}
