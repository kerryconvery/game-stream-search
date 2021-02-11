using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application;
using GameStreamSearch.Types;
using System;
using GameStreamSearch.StreamProviders.Gateways;
using GameStreamSearch.StreamProviders.Mappers;

namespace GameStreamSearch.StreamProviders
{
    public class DLiveStreamProvider : IStreamProvider
    {
        private readonly DLiveGraphQLGateway dliveApi;
        private readonly DLiveMapper mapper;

        public DLiveStreamProvider(DLiveGraphQLGateway dliveApi, DLiveMapper mapper)
        {
            this.dliveApi = dliveApi;
            this.mapper = mapper;
        }

        public async Task<Streams> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            if (!AreFilterOptionsSupports(filterOptions))
            {
                throw new ArgumentException("The Dlive platform does not support these filter options");
            };

            var pageOffset = new NumericPageOffset(pageSize, pageToken);

            var liveStreamsResult = await dliveApi.GetLiveStreams(pageSize, pageOffset, StreamSortOrder.Trending);

            return liveStreamsResult
                .Select(streams => mapper.ToGameStreamsDto(streams, pageOffset.GetNextOffset()))
                .GetOrElse(Streams.Empty);
        }

        public async Task<MaybeResult<PlatformChannel, StreamProviderError>> GetStreamerChannel(string channelName)
        {
            var userResult = await dliveApi.GetUserByDisplayName(channelName);

            return userResult
                .Select(mapper.ToStreamerChannelDto);
        }

        public bool AreFilterOptionsSupports(StreamFilterOptions filterOptions)
        {
            return string.IsNullOrEmpty(filterOptions.GameName);
        }

        public StreamPlatformType Platform => StreamPlatformType.DLive;
    }
}
