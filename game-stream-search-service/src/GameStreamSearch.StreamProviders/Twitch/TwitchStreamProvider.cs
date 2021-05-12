using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.StreamProviders.Twitch.Gateways;
using GameStreamSearch.StreamProviders.Twitch.Mappers;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.Common;
using System;

namespace GameStreamSearch.StreamProviders.Twitch
{
    public class TwitchStreamProvider : IStreamProvider
    {
        private readonly TwitchStreamGateway streamGateway;
        private readonly TwitchChannelGateway channelGateway;
        private readonly TwitchStreamMapper streamMapper;
        private readonly TwitchChannelMapper channelMapper;

        public TwitchStreamProvider(
            TwitchStreamGateway streamGateway,
            TwitchChannelGateway channelGateway,
            TwitchStreamMapper streamMapper,
            TwitchChannelMapper channelMapper
        )
        {
            this.streamGateway = streamGateway;
            this.channelGateway = channelGateway;
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
                return streamGateway.GetLiveStreams(pageSize, pageOffset);
            }

            return streamGateway.SearchStreams(filterOptions.GameName, pageSize, pageOffset);
        }

        public async Task<Maybe<PlatformChannelDto>> GetStreamerChannel(string channelName)
        {
            var channel = await channelGateway.GetChannelByName(channelName);

            return channel.Select(channelMapper.Map);
        }

        public string StreamPlatformName => StreamPlatform.Twitch;
    }
}
