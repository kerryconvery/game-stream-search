using System;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Application.ValueObjects;

namespace GameStreamSearch.Application.Queries
{
    public class StreamsQuery
    {
        public StreamFilterOptions FilterOptions { get; init; }
        public string PageToken { get; init; }
        public int PageSize { get; init; }
    }

    public class GetStreamsQueryHandler : IQueryHandler<StreamsQuery, AggregatedStreamsDto>
    {
        private readonly StreamProviderService streamProviderService;
        private readonly PageTokenService pageTokenService;

        public GetStreamsQueryHandler(StreamProviderService streamProviderService, PageTokenService pageTokenService)
        {
            this.streamProviderService = streamProviderService;
            this.pageTokenService = pageTokenService;
        }

        public async Task<AggregatedStreamsDto> Execute(StreamsQuery query)
        {
            var pageTokens = pageTokenService.UnpackPageToken(query.PageToken);

            var streamSources = streamProviderService.CreateStreamSources(pageTokens, query.FilterOptions);

            var platformStreams = await streamProviderService.GetStreams(streamSources, query.FilterOptions, query.PageSize);

            return new AggregatedStreamsDto
            {
                Streams = platformStreams.SelectMany(p => p.Streams.Select(s => new StreamDto
                {
                    StreamTitle = s.StreamTitle,
                    StreamerName = s.StreamerName,
                    StreamUrl = s.StreamUrl,
                    IsLive = s.IsLive,
                    Views = s.Views,
                    PlatformDisplayName = p.StreamPlatform.PlatformDisplayName,
                    StreamThumbnailUrl = s.StreamThumbnailUrl,
                    StreamerAvatarUrl = s.StreamerAvatarUrl,
                })),
                NextPageToken = pageTokenService.PackPageTokens(
                    platformStreams.ToDictionary(s => s.StreamPlatform.PlatformId, s => s.NextPageToken))
            };
        }
    }
}
