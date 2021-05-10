using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Services.StreamProvider;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.Common;

namespace GameStreamSearch.Application.GetStreams
{
    public class GetStreamsQuery
    {
        public StreamFilterOptions Filters { get; init; }
        public string PageToken { get; init; }
        public int PageSize { get; init; }
    }

    public class Stream
    {
        public string StreamTitle { get; init; }
        public string StreamThumbnailUrl { get; init; }
        public string StreamUrl { get; init; }
        public string StreamerName { get; init; }
        public string StreamerAvatarUrl { get; init; }
        public string PlatformName { get; set; }
        public bool IsLive { get; init; }
        public int Views { get; init; }

        public static Stream Create(string platformName, PlatformStreamDto platformStream)
        {
            return new Stream
            {
                StreamTitle = platformStream.StreamTitle,
                StreamerName = platformStream.StreamerName,
                StreamUrl = platformStream.StreamUrl,
                IsLive = platformStream.IsLive,
                Views = platformStream.Views,
                PlatformName = platformName,
                StreamThumbnailUrl = platformStream.StreamThumbnailUrl,
                StreamerAvatarUrl = platformStream.StreamerAvatarUrl,
            };
        }
    }

    public class GetStreamsQueryResponseDto
    {
        public IEnumerable<Stream> Streams { get; init; }
        public string NextPageToken { get; init; }
    }

    public class GetStreamsQueryHandler : IQueryHandler<GetStreamsQuery, GetStreamsQueryResponseDto>
    {
        private readonly StreamPlatformService streamPlatformService;

        public GetStreamsQueryHandler(StreamPlatformService streamPlatformService)
        {
            this.streamPlatformService = streamPlatformService;
        }

        public async Task<GetStreamsQueryResponseDto> Execute(GetStreamsQuery query)
        {
            var unpackedTokens = PageTokens.UnpackTokens(query.PageToken);

            var platformStreams = await streamPlatformService.GetStreams(query.Filters, query.PageSize, unpackedTokens);

            var packedTokens = PackPageTokens(platformStreams);

            var aggregatedStreams = AggregatePlatformStreams(platformStreams);

            return new GetStreamsQueryResponseDto
            {
                Streams = aggregatedStreams.OrderByDescending(s => s.Views),
                NextPageToken = packedTokens,
            };
        }

        private string PackPageTokens(IEnumerable<PlatformStreamsDto> platformStreams)
        {
            return PageTokens
                .FromList(platformStreams.Select(p => new PageToken(p.StreamPlatformName, p.NextPageToken)))
                .PackTokens();
        }

        private IEnumerable<Stream> AggregatePlatformStreams(IEnumerable<PlatformStreamsDto> platformStreams)
        {
            return platformStreams.SelectMany(platform => platform.Streams.Select(stream => Stream.Create(platform.StreamPlatformName, stream)));
        }
    }
}
