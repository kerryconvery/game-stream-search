using System.Threading.Tasks;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Types;
using System;
using GameStreamSearch.Application.Mappers;
using GameStreamSearch.Domain.Entities;
using GameStreamSearch.Gateways;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Application
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

        public StreamPlatform StreamPlatform => StreamPlatform.DLive;
    }
}
