﻿using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.StreamProviders.YouTube.Gateways.V3;
using GameStreamSearch.StreamProviders.YouTube.Mappers.V3;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.Common;
using System;
using GameStreamSearch.StreamProviders.YouTube.Gateways.Dto.V3;
using System.Collections.Generic;
using GameStreamSearch.StreamProviders.YouTube.Gateways;

namespace GameStreamSearch.StreamProviders.YouTube
{
    public class YouTubeStreamProvider : IStreamProvider
    {
        private readonly YouTubeChannelGateway channelGateway;
        private readonly YouTubeV3Gateway youTubeV3Api;
        private readonly YouTubeStreamMapper streamMapper;
        private readonly YouTubeChannelMapper channelMapper;

        public YouTubeStreamProvider(
            YouTubeChannelGateway channelGateway,
            YouTubeV3Gateway youTubeV3Api,
            YouTubeStreamMapper streamMapper,
            YouTubeChannelMapper channelMapper
        )
        {
            this.channelGateway = channelGateway;
            this.youTubeV3Api = youTubeV3Api;
            this.streamMapper = streamMapper;
            this.channelMapper = channelMapper;
        }

        public async Task<PlatformStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, PageToken pageToken)
        {
            try
            {
                return await TryGetLivePlatformStreams(filterOptions, pageSize, pageToken);
            } catch(Exception)
            {
                return PlatformStreamsDto.Empty(StreamPlatformName);
            }
        }

        private async Task<PlatformStreamsDto> TryGetLivePlatformStreams(StreamFilterOptions filterOptions, int pageSize, PageToken pageToken)
        {
            var searchResults = await youTubeV3Api.SearchGamingVideos(
                filterOptions.GameName, VideoEventType.Live, VideoSortType.ViewCount, pageSize, pageToken);

            var taskGetVideos = GetVideosForSearchItems(searchResults.items);
            var taskGetChannels = GetChannelsForSearchItems(searchResults.items);

            var videos = await taskGetVideos;
            var channels = await taskGetChannels;

            return streamMapper.Map(searchResults, videos, channels);
        }

        private Task<IEnumerable<YouTubeVideoDto>> GetVideosForSearchItems(IEnumerable<YouTubeSearchItemDto> videoSearchItems)
        {
            var videoIds = videoSearchItems.Select(v => v.id.videoId);

            return youTubeV3Api.GetVideos(videoIds.ToArray());
        }

        private Task<IEnumerable<YouTubeChannelDto>> GetChannelsForSearchItems(IEnumerable<YouTubeSearchItemDto> videoSearchItems)
        {
            var channelIds = videoSearchItems.Select(v => v.snippet.channelId);

            return channelGateway.BulkGetAvartarByChannelId(channelIds.ToArray());
        }

        public async Task<Maybe<PlatformChannelDto>> GetStreamerChannel(string channelName)
        {
            var channel = await channelGateway.GetChannelByName(channelName);

            return channel.Select(channelMapper.Map);
        }

        public string StreamPlatformName => StreamPlatform.YouTube;
    }
}
