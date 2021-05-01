using System.Threading.Tasks;
using GameStreamSearch.Types;
using System;
using GameStreamSearch.StreamProviders.DLive.Gateways;
using GameStreamSearch.StreamProviders.DLive.Mappers;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.Common;

namespace GameStreamSearch.StreamProviders.DLive
{
    public class DLiveStreamProvider : IStreamProvider
    {
        private readonly DLiveGraphQLGateway dliveApi;
        private readonly DLiveStreamMapper streamMapper;
        private readonly DLiveChannelMapper channelMapper;

        public DLiveStreamProvider(
            DLiveGraphQLGateway dliveApi,
            DLiveStreamMapper streamMapper,
            DLiveChannelMapper channelMapper
       )
        {
            this.dliveApi = dliveApi;
            this.streamMapper = streamMapper;
            this.channelMapper = channelMapper;
        }

        public async Task<PlatformStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, PageToken pageToken)
        {
            if (!AreFilterOptionsSupported(filterOptions))
            {
                throw new ArgumentException("The Dlive platform does not support these filter options");
            };

            var liveStreamsResult = await dliveApi.GetLiveStreams(pageSize, pageToken, StreamSortOrder.Trending);

            return streamMapper.Map(liveStreamsResult, pageSize, pageToken);
        }

        public async Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string channelName)
        {
            var userResult = await dliveApi.GetUserByDisplayName(channelName);

            return channelMapper.Map(userResult);
        }

        public bool AreFilterOptionsSupported(StreamFilterOptions filterOptions)
        {
            return string.IsNullOrEmpty(filterOptions.GameName);
        }

        public string StreamPlatformName => StreamPlatform.DLive;
    }
}
