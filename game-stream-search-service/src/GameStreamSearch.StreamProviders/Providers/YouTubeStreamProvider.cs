using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamProviders.Dto.YouTube.YouTubeV3;
using GameStreamSearch.Application;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders
{
    public class YouTubeStreamProvider : IStreamProvider
    {
        private readonly string youTubeWebUrl;
        private readonly IYouTubeV3Api youTubeV3Api;

        public YouTubeStreamProvider(string youTubeWebUrl, IYouTubeV3Api youTubeV3Api)
        {
            this.youTubeWebUrl = youTubeWebUrl;
            this.youTubeV3Api = youTubeV3Api;
        }

        private IEnumerable<GameStreamDto> MapAsLiveStream(
            IEnumerable<YouTubeSearchItemDto> streams,
            Dictionary<string, YouTubeChannelSnippetDto> channelSnippets,
            Dictionary<string, YouTubeVideoLiveStreamingDetailsDto> liveStreamDetails)
        {
            var gameStreams = streams.Select(v => {
                var viewers = liveStreamDetails.ContainsKey(v.id.videoId) ? liveStreamDetails[v.id.videoId].concurrentViewers : 0;
                var avatarUrl = channelSnippets.ContainsKey(v.snippet.channelId) ? channelSnippets[v.snippet.channelId].thumbnails.@default.url : null;

                return new GameStreamDto
                {
                    StreamerName = v.snippet.channelTitle,
                    StreamTitle = v.snippet.title,
                    StreamThumbnailUrl = v.snippet.thumbnails.medium.url,
                    StreamerAvatarUrl = avatarUrl,
                    StreamUrl = $"{youTubeWebUrl}/watch?v={v.id.videoId}",
                    StreamPlatformName = Platform.GetFriendlyName(),
                    IsLive = true,
                    Views = viewers,
                };
            });

            return gameStreams;
        }

        private async Task<Result<Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>, YouTubeErrorType>> GetLiveStreamDetails(
            IEnumerable<YouTubeSearchItemDto> streams)
        {
            var videoIds = streams.Select(v => v.id.videoId).ToArray();

            var videosResult = await youTubeV3Api.GetVideos(videoIds);

            if (videosResult.IsFailure)
            {
                return Result<Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>, YouTubeErrorType>.Fail(YouTubeErrorType.ProviderNotAvailable);
            }

            var streamDetails = videosResult.Value
                    .Map(videos => videos.ToDictionary(v => v.id, v => v.liveStreamingDetails))
                    .GetOrElse(new Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>());

            return Result<Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>, YouTubeErrorType>.Success(streamDetails);
        }

        private async Task<Result<Dictionary<string, YouTubeChannelSnippetDto>, YouTubeErrorType>> GetChannelSnippets(
            IEnumerable<YouTubeSearchItemDto> streams)
        {
            var channelIds = streams.Select(v => v.snippet.channelId).ToArray();

            var channelsResults = await youTubeV3Api.GetChannels(channelIds);

            if (channelsResults.IsFailure)
            {
                return Result<Dictionary<string, YouTubeChannelSnippetDto>, YouTubeErrorType>.Fail(YouTubeErrorType.ProviderNotAvailable);
            }

            var channelSnippets = channelsResults.Value
                    .Map(channels => channels.ToDictionary(c => c.id, c => c.snippet))
                    .GetOrElse(new Dictionary<string, YouTubeChannelSnippetDto>());

            return Result<Dictionary<string, YouTubeChannelSnippetDto>, YouTubeErrorType>.Success(channelSnippets);
        }

        public async Task<GameStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            var liveVideosResult = await youTubeV3Api.SearchGamingVideos(
                filterOptions.GameName, VideoEventType.Live, VideoSortType.ViewCount, pageSize, pageToken);

            if (liveVideosResult.IsFailure || liveVideosResult.Value.IsNothing)
            {
                return GameStreamsDto.Empty;
            }

            var liveVideos = liveVideosResult.Value.Unwrap();

            var getChannelSnippetsTask = GetChannelSnippets(liveVideos.items);
            var getLiveStreamDetailsTask = GetLiveStreamDetails(liveVideos.items);

            var channelSnippets = await getChannelSnippetsTask;
            var liveStreamDetails = await getLiveStreamDetailsTask;

            if (channelSnippets.IsFailure || liveStreamDetails.IsFailure)
            {
                return GameStreamsDto.Empty;
            }

            return new GameStreamsDto
            {
                Items = MapAsLiveStream(liveVideos.items, channelSnippets.Value, liveStreamDetails.Value),
                NextPageToken = liveVideos.nextPageToken,
            };
        }

        public async Task<MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>> GetStreamerChannel(string channelName)
        {
            var channelsResult = await youTubeV3Api.SearchChannelsByUsername(channelName, 1);

            if (channelsResult.IsFailure)
            {
                return MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Fail(GetStreamerChannelErrorType.ProviderNotAvailable);
            }

            var channel = channelsResult.Value.Map(channels => channels
                .Select(channel => channel.ToStreamerChannelDto(youTubeWebUrl))
                .FirstOrDefault()
            );

            return MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(channel);
        }

        public StreamPlatformType Platform => StreamPlatformType.YouTube;
    }
}
