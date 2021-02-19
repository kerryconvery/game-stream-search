using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Services;

namespace GameStreamSearch.Application.Queries
{
    public class StreamsQuery
    {
        public StreamFilterOptions FilterOptions { get; init; }
        public string PageToken { get; init; }
        public int PageSize { get; init; }
    }

    public class GetStreamsQueryHandler : IQueryHandler<StreamsQuery, string>
    {
        private readonly StreamProviderService streamProviderService;
        private readonly StreamAggregationService streamAggregationService;

        public GetStreamsQueryHandler(StreamProviderService streamProviderService, StreamAggregationService streamAggregationService)
        {
            this.streamProviderService = streamProviderService;
            this.streamAggregationService = streamAggregationService;
        }

        public async Task<string> Execute(StreamsQuery query)
        {
            var pageTokens = streamAggregationService.UnpackPageToken(query.PageToken);

            var streamSources = streamProviderService.CreateStreamSources(pageTokens, query.FilterOptions);

            var platformStreams = await streamProviderService.GetStreams(streamSources, query.FilterOptions, query.PageSize);

            return streamAggregationService.AggregateStreams(platformStreams);
        }
    }
}
