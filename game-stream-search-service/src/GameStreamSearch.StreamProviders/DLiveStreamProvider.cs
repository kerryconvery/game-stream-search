using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
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
        private readonly string streamPlatformId;
        private readonly DLiveGraphQLGateway dliveApi;
        private readonly DLiveStreamMapper streamMapper;
        private readonly DLiveChannelMapper channelMapper;

        public DLiveStreamProvider(
            string streamPlatformId,
            DLiveGraphQLGateway dliveApi,
            DLiveStreamMapper streamMapper,
            DLiveChannelMapper channelMapper
       )
        {
            this.streamPlatformId = streamPlatformId;
            this.dliveApi = dliveApi;
            this.streamMapper = streamMapper;
            this.channelMapper = channelMapper;
        }

        public async Task<PlatformStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            if (!AreFilterOptionsSupported(filterOptions))
            {
                throw new ArgumentException("The Dlive platform does not support these filter options");
            };

            var pageOffset = int.Parse(pageToken);

            var liveStreamsResult = await dliveApi.GetLiveStreams(pageSize, pageOffset, StreamSortOrder.Trending);

            return streamMapper.Map(StreamPlatformId, liveStreamsResult, pageSize, pageOffset);
        }

        public async Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string channelName)
        {
            var userResult = await dliveApi.GetUserByDisplayName(channelName);

            return channelMapper.Map(StreamPlatformId, userResult);
        }

        public bool AreFilterOptionsSupported(StreamFilterOptions filterOptions)
        {
            return string.IsNullOrEmpty(filterOptions.GameName);
        }

        public string StreamPlatformId => streamPlatformId;
    }
}
