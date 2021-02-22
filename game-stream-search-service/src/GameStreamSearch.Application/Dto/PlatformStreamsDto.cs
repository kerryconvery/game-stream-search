using System.Collections.Generic;

namespace GameStreamSearch.Application.Models
{
    public class PlatformStreamsDto
    {
        public string StreamPlatformName { get; init; }
        public IEnumerable<PlatformStreamDto> Streams { get; init;  }
        public string NextPageToken { get; init; }

        public static PlatformStreamsDto Empty(string streamPlatformName) {
            return new PlatformStreamsDto
            {
                StreamPlatformName = streamPlatformName,
                Streams = new List<PlatformStreamDto>(),
                NextPageToken = string.Empty
            };
        }
    }
}
