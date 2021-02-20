using System.Collections.Generic;

namespace GameStreamSearch.Application.ValueObjects
{
    public class PlatformStreams
    {
        public StreamPlatform StreamPlatform { get; init; }
        public IEnumerable<PlatformStream> Streams { get; init;  }
        public string NextPageToken { get; init; }

        public static PlatformStreams Empty(StreamPlatform streamPlatform) {
            return new PlatformStreams
            {
                StreamPlatform = streamPlatform,
                Streams = new List<PlatformStream>(),
                NextPageToken = string.Empty
            };
        }
    }
}
