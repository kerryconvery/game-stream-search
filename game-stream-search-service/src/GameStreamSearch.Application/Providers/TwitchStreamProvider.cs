using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Application.Mappers;
using GameStreamSearch.Application.Selectors;
using GameStreamSearch.Types;
using GameStreamSearch.Domain.Entities;
using GameStreamSearch.Gateways;
using GameStreamSearch.Gateways.Dto.Twitch.Kraken;
using GameStreamSearch.Application.Dto;

namespace GameStreamSearch.Application
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
            var liveStreamsResult = await FindLiveStreams(filterOptions, pageSize, pageToken);

            return streamMapper.Map(liveStreamsResult, pageSize, pageToken);
        }

        private async Task<MaybeResult<IEnumerable<TwitchStreamDto>, StreamProviderError>> FindLiveStreams(
            StreamFilterOptions filterOptions, int pageSize, int pageOffset)
        {
            if (string.IsNullOrEmpty(filterOptions.GameName))
            {
                return await twitchStreamApi.GetLiveStreams(pageSize, pageOffset);
            }

            return await twitchStreamApi.SearchStreams(filterOptions.GameName, pageSize, pageOffset);
        }

        public async Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string channelName)
        {
            var channelsResult = await twitchStreamApi.SearchChannels(channelName, 1, 0);

            return channelMapper.Map(TwitchChannelSelector.Select(channelName, channelsResult));
        }

        public StreamPlatform StreamPlatform => StreamPlatform.Twitch;
    }
}
