using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamProviders.Dto.Twitch.Kraken;
using GameStreamSearch.Types;
using GameStreamSearch.StreamProviders.Mappers;
using GameStreamSearch.StreamProviders.Gateways;

namespace GameStreamSearch.StreamProviders
{
    public class TwitchStreamProvider : IStreamProvider
    {
        private readonly TwitchKrakenGateway twitchStreamApi;
        private readonly TwitchMapper twitchMapper;

        public TwitchStreamProvider(TwitchKrakenGateway twitchStreamApi, TwitchMapper twitchMapper)
        {
            this.twitchStreamApi = twitchStreamApi;
            this.twitchMapper = twitchMapper;
        }

        public async Task<Streams> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            var pageOffset = new NumericPageOffset(pageSize, pageToken);

            var liveStreamsResult = await GetLiveStreams(filterOptions, pageSize, pageOffset);

            return liveStreamsResult
                .Select(streams => twitchMapper.ToGameStreamsDto(streams, pageOffset.GetNextOffset()))
                .GetOrElse(Streams.Empty);
        }

        private async Task<MaybeResult<IEnumerable<TwitchStreamDto>, StreamProviderError>> GetLiveStreams(
            StreamFilterOptions filterOptions, int pageSize, NumericPageOffset pageOffset)
        {
            if (string.IsNullOrEmpty(filterOptions.GameName))
            {
                return await twitchStreamApi.GetLiveStreams(pageSize, pageOffset);
            }

            return await twitchStreamApi.SearchStreams(filterOptions.GameName, pageSize, pageOffset);
        }

        public async Task<MaybeResult<PlatformChannel, StreamProviderError>> GetStreamerChannel(string channelName)
        {
            var channelsResult = await twitchStreamApi.SearchChannels(channelName, 1, 0);

           return channelsResult
                .Select(channels => twitchMapper.ToStreamerChannelDto(channels, channelName));
        }

        public StreamPlatformType Platform => StreamPlatformType.Twitch;
    }
}
