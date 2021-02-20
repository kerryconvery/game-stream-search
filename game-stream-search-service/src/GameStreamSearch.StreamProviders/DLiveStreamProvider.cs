﻿using System.Threading.Tasks;
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

        public async Task<PlatformStreams> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            if (!AreFilterOptionsSupported(filterOptions))
            {
                throw new ArgumentException("The Dlive platform does not support these filter options");
            };

            var pageOffset = int.Parse(pageToken);

            var liveStreamsResult = await dliveApi.GetLiveStreams(pageSize, pageOffset, StreamSortOrder.Trending);

            return streamMapper.Map(liveStreamsResult, pageSize, pageOffset);
        }

        public async Task<MaybeResult<PlatformChannel, StreamProviderError>> GetStreamerChannel(string channelName)
        {
            var userResult = await dliveApi.GetUserByDisplayName(channelName);

            return channelMapper.Map(userResult);
        }

        public bool AreFilterOptionsSupported(StreamFilterOptions filterOptions)
        {
            return string.IsNullOrEmpty(filterOptions.GameName);
        }

        public StreamPlatform StreamPlatform => StreamPlatformType.DLive;
    }
}
