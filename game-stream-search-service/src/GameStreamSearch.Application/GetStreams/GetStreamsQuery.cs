using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application.GetStreams
{
    public class StreamFilters
    {
        public string GameName { get; init; }
    }

    public class GetStreamsQuery
    {
        public IEnumerable<string> StreamPlatformNames { get; init; }
        public StreamFilters Filters { get; init; }
        public string PageToken { get; init; }
        public int PageSize { get; init; }
    }

}
