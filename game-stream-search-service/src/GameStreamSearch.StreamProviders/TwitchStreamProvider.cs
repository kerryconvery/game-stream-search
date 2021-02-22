using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Types;
using GameStreamSearch.StreamProviders.Dto.Twitch.Kraken;
using GameStreamSearch.StreamProviders.Gateways;
using GameStreamSearch.StreamProviders.Mappers;
using GameStreamSearch.StreamProviders.Selectors;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders
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

        public async Task<PlatformStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            var pageOffset = int.Parse(pageToken);

            var liveStreamsResult = await GetLiveStreams(filterOptions, pageSize, pageOffset);

            return streamMapper.Map(liveStreamsResult, pageSize, pageOffset);
        }

        private async Task<MaybeResult<IEnumerable<TwitchStreamDto>, StreamProviderError>> GetLiveStreams(
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
