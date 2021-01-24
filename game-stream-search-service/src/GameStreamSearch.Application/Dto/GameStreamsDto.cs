using System;
using System.Collections.Generic;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Dto
{
    public class GameStreamDto
    {
        public string StreamTitle { get; set; }
        public string StreamThumbnailUrl { get; set; }
        public string StreamUrl { get; set; }
        public string StreamerName { get; set; }
        public string StreamerAvatarUrl { get; set; }
        public bool IsLive { get; set; }
        public int Views { get; set; }
        public string StreamPlatformName { get; set; }
    }

    public class GameStreamsDto
    {
        public IEnumerable<GameStreamDto> Items { get; set; }
        public string NextPageToken { get; set; }

        public static GameStreamsDto Empty => new GameStreamsDto
        {
            Items = new List<GameStreamDto>(),
            NextPageToken = null,
        };

    }
}
