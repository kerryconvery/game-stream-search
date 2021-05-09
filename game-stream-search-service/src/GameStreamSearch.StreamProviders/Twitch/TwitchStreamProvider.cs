using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.StreamProviders.Twitch.Gateways;
using GameStreamSearch.StreamProviders.Twitch.Mappers;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;
using GameStreamSearch.StreamProviders.Twitch.Selectors;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.Common;
using System;

namespace GameStreamSearch.StreamProviders.Twitch
{
    public class TwitchStreamProvider : IStreamProvider
    {
        private readonly TwitchKrakenGateway twitchStreamApi;
        private readonly TwitchStreamMapper streamMapper;
        private readonly TwitchChannelMapper channelMapper;

        public TwitchStreamProvider(
            TwitchKrakenGateway twitchStreamApi,
            TwitchStreamMapper streamMapper,
            TwitchChannelMapper channelMapper
        )
        {
            this.twitchStreamApi = twitchStreamApi;
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
        
        private async Task<PlatformStreamsDto> TryGetLivePlatformStreams(StreamFilterOptions filterOptions, int pageSize, int pageOffset)
        {
            var liveStreams = await GetLiveStreamsWithFilter(filterOptions, pageSize, pageOffset);

            return streamMapper.Map(liveStreams, pageSize, pageOffset);
        }

        private Task<IEnumerable<TwitchStreamDto>> GetLiveStreamsWithFilter(StreamFilterOptions filterOptions, int pageSize, int pageOffset)
        {
            if (string.IsNullOrEmpty(filterOptions.GameName))
            {
                return twitchStreamApi.GetLiveStreams(pageSize, pageOffset);
            }

            return twitchStreamApi.SearchStreams(filterOptions.GameName, pageSize, pageOffset);
        }

        public async Task<Maybe<PlatformChannelDto>> GetStreamerChannel(string channelName)
        {
            var channels = await twitchStreamApi.SearchChannels(channelName, 1, 0);

            return channelMapper.Map(TwitchChannelSelector.Select(channelName, channels));
        }

        public string StreamPlatformName => StreamPlatform.Twitch;
    }
}
