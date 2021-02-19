using System.Collections.Generic;

namespace GameStreamSearch.Application.Dto
{
    public class PlatformStreamsDto
    {
        public string StreamPlatformId { get; init; }
        public IEnumerable<PlatformStreamDto> Streams { get; init;  }
        public string NextPageToken { get; init; }

        public static PlatformStreamsDto Empty(string streamPlatformId) {
            return new PlatformStreamsDto
            {
                StreamPlatformId = streamPlatformId,
                Streams = new List<PlatformStreamDto>(),
                NextPageToken = string.Empty
            };
        }
    }
}
