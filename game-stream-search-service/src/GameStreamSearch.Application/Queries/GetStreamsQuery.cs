using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Services;
using GameStreamSearch.Application.Models;

namespace GameStreamSearch.Application.Queries
{
    public class StreamsQuery
    {
        public IEnumerable<string> StreamPlatformNames { get; init; }
        public StreamFilterOptions FilterOptions { get; init; }
        public string PageToken { get; init; }
        public int PageSize { get; init; }
    }

    public class GetStreamsQueryHandler : IQueryHandler<StreamsQuery, AggregatedStreamsDto>
    {
        private readonly StreamProviderService streamProviderService;

        public GetStreamsQueryHandler(StreamProviderService streamProviderService)
        {
            this.streamProviderService = streamProviderService;
        }

        public async Task<AggregatedStreamsDto> Execute(StreamsQuery query)
        {
            var unpackedTokens = PageTokens.UnpackTokens(query.PageToken);

            var supportedPlatforms = streamProviderService.GetSupportingPlatforms(query.FilterOptions);

            var platformStreams = await streamProviderService.GetStreams(supportedPlatforms, query.FilterOptions, query.PageSize, unpackedTokens);

            var packedTokens = PageTokens
                .FromList(platformStreams.Select(p => new PageToken(p.StreamPlatformName, p.NextPageToken)))
                .PackTokens();

            var aggregatedStreams = platformStreams.SelectMany(p => p.Streams.Select(s => new StreamDto
                {
                    StreamTitle = s.StreamTitle,
                    StreamerName = s.StreamerName,
                    StreamUrl = s.StreamUrl,
                    IsLive = s.IsLive,
                    Views = s.Views,
                    PlatformName = p.StreamPlatformName,
                    StreamThumbnailUrl = s.StreamThumbnailUrl,
                    StreamerAvatarUrl = s.StreamerAvatarUrl,
                }));

            return new AggregatedStreamsDto
            {
                Streams = aggregatedStreams.OrderByDescending(s => s.Views),
                NextPageToken = packedTokens
            };
        }
    }
}
