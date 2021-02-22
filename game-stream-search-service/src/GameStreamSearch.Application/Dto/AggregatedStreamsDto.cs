using System.Collections.Generic;

namespace GameStreamSearch.Application.Models
{
    public class AggregatedStreamsDto
    {
        public IEnumerable<StreamDto> Streams { get; init; }
        public string NextPageToken { get; init; }
    }
}
