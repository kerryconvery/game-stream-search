using System;
using System.Collections.Generic;
using System.Linq;

namespace GameStreamSearch.Application.ValueObjects
{
    public class Stream
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

    public class Streams
    {
        private readonly string nextPageToken;

        public Streams(IEnumerable<Stream> items, string nextPageToken)
        {
            Items = items;
            this.nextPageToken = nextPageToken;
        }

        public IEnumerable<Stream> Items { get; }

        public string NextPageToken => Items.Count() > 0 ? nextPageToken : string.Empty;

        public static Streams Empty => new Streams(new List<Stream>(), null);
    }
}
