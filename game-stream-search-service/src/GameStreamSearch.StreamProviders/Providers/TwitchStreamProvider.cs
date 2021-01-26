using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamProviders.Dto.Twitch.Kraken;
using GameStreamSearch.Types;
using GameStreamSearch.StreamProviders.Providers;

namespace GameStreamSearch.StreamProviders
{
    public class TwitchStreamProvider : StreamProvider, IStreamProvider
    {
        private readonly ITwitchKrakenApi twitchStreamApi;

        public TwitchStreamProvider(ITwitchKrakenApi twitchStreamApi)
        {
            this.twitchStreamApi = twitchStreamApi;
        }

        private GameStreamDto MapToGameStream(TwitchStreamDto liveStream)
        {
            return new GameStreamDto
            {
                StreamTitle = liveStream.channel.status,
                StreamerName = liveStream.channel.display_name,
                StreamerAvatarUrl = liveStream.channel.logo,
                StreamThumbnailUrl = liveStream.preview.medium,
                StreamUrl = liveStream.channel.url,
                StreamPlatformName = Platform.GetFriendlyName(),
                IsLive = true,
                Views = liveStream.viewers,
            };
        }

        private async Task<MaybeResult<IEnumerable<TwitchStreamDto>, TwitchErrorType>> GetLiveStreams(
            StreamFilterOptions filterOptions, int pageSize, int pageOffset)
        {
            if (string.IsNullOrEmpty(filterOptions.GameName))
            {
                return await twitchStreamApi.GetLiveStreams(pageSize, pageOffset);
            }
            else
            {
                return await twitchStreamApi.SearchStreams(filterOptions.GameName, pageSize, pageOffset);
            }
        }

        public async Task<GameStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            var pageOffset = GetPageOffset(pageToken);

            var liveStreamsResult = await GetLiveStreams(filterOptions, pageSize, pageOffset);

            if (liveStreamsResult.IsFailure)
            {
                return GameStreamsDto.Empty;
            }

            return liveStreamsResult.Value.Map(liveStreams =>
            {
                var nextPageToken = GetNextPageToken(liveStreams.Any(), pageSize, pageOffset);

                return new GameStreamsDto
                {
                    Items = liveStreams.Select(MapToGameStream),
                    NextPageToken = nextPageToken
                };
            }).GetOrElse(GameStreamsDto.Empty);
        }

        public async Task<MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>> GetStreamerChannel(string channelName)
        {
            var channelsResult = await twitchStreamApi.SearchChannels(channelName, 1, 0);

            if (channelsResult.IsFailure)
            {
                return MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Fail(GetStreamerChannelErrorType.ProviderNotAvailable);
            }

            var streamerChannel = channelsResult.Value.Map(result => result.Channels
                .Where(channel => channel.display_name.Equals(channelName, System.StringComparison.CurrentCultureIgnoreCase))
                .Select(channel => channel.ToStreamerChannelDto())
                .FirstOrDefault()
            );

            return MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(streamerChannel);
        }

        public StreamPlatformType Platform => StreamPlatformType.Twitch;
    }
}
