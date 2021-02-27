using System.Collections.Generic;

namespace GameStreamSearch.Application.Dto
{
    public class AggregatedStreamsDto
    {
        public IEnumerable<StreamDto> Streams { get; init; }
        public string NextPageToken { get; init; }
    }
}
