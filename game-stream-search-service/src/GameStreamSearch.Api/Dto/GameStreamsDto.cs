using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application.Dto
{
    public class GameStreamDto
    {
        public string StreamTitle { get; init; }
        public string StreamThumbnailUrl { get; init; }
        public string StreamUrl { get; init; }
        public string StreamerName { get; init; }
        public string StreamerAvatarUrl { get; init; }
        public bool IsLive { get; init; }
        public int Views { get; init; }
        public string StreamPlatformName { get; init; }
    }

    public class GameStreamsDto
    {
        public IEnumerable<GameStreamDto> Items { get; init; }
        public string NextPageToken { get; init; }

        public static GameStreamsDto Empty()
        {
            return new GameStreamsDto
            {
                Items = new List<GameStreamDto>(),
                NextPageToken = null,
            };
        }

    }
}
