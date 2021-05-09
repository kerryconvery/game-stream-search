﻿using System.Threading.Tasks;
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
            try
            {
                return await TryGetLivePlatformStreams(filterOptions, pageSize, pageToken);
            } catch(ArgumentException)
            {
                throw;
            } catch(Exception)
            {
                return PlatformStreamsDto.Empty(StreamPlatformName);
            }
        }

        private async Task<PlatformStreamsDto> TryGetLivePlatformStreams(StreamFilterOptions filterOptions, int pageSize, PageToken pageToken)
        {
            if (!AreFilterOptionsSupported(filterOptions))
            {
                throw new ArgumentException("The DLive platform does not support these filter options");
            };

            var liveStreams = await dliveApi.GetLiveStreams(pageSize, pageToken, StreamSortOrder.Trending);

            return streamMapper.Map(liveStreams, pageSize, pageToken);
        }

        public async Task<Maybe<PlatformChannelDto>> GetStreamerChannel(string channelName)
        {
            var maybeUser = await dliveApi.GetUserByDisplayName(channelName);

            return maybeUser.Select(channelMapper.Map);
        }

        public bool AreFilterOptionsSupported(StreamFilterOptions filterOptions)
        {
            return string.IsNullOrEmpty(filterOptions.GameName);
        }

        public string StreamPlatformName => StreamPlatform.DLive;
    }
}
