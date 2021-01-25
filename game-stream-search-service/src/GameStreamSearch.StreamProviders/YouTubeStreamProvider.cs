using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamPlatformApi;
using GameStreamSearch.StreamPlatformApi.YouTube.Dto.YouTubeV3;
using GameStreamSearch.Application;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders
{
    public class YouTubeStreamProvider : IStreamProvider
    {
        private readonly string youTubeBaseUrl;
        private readonly IYouTubeV3Api youTubeV3Api;

        public YouTubeStreamProvider(string youTubeBaseUrl, IYouTubeV3Api youTubeV3Api)
        {
            this.youTubeBaseUrl = youTubeBaseUrl;
            this.youTubeV3Api = youTubeV3Api;
        }

        private IEnumerable<GameStreamDto> mapAsLiveStream(
            IEnumerable<YouTubeSearchItemDto> streams,
            Dictionary<string, YouTubeChannelSnippetDto> channelSnippets,
            Dictionary<string, YouTubeVideoLiveStreamingDetailsDto> liveStreamDetails)
        {
            var gameStreams = streams.Select(v => {
                var streamDetails = liveStreamDetails.ContainsKey(v.id.videoId) ? liveStreamDetails[v.id.videoId] : null;
                var channelSnippet = channelSnippets.ContainsKey(v.snippet.channelId) ? channelSnippets[v.snippet.channelId] : null;

                return new GameStreamDto
                {
                    StreamerName = v.snippet.channelTitle,
                    StreamTitle = v.snippet.title,
                    StreamThumbnailUrl = v.snippet.thumbnails.medium.url,
                    StreamerAvatarUrl = channelSnippet?.thumbnails.@default.url,
                    StreamUrl = $"{youTubeBaseUrl}/watch?v={v.id.videoId}",
                    StreamPlatformName = Platform.GetFriendlyName(),
                    IsLive = true,
                    Views = streamDetails != null ? streamDetails.concurrentViewers : 0,
                };
            });

            return gameStreams;
        }

        private async Task<Result<Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>, YoutubeErrorType>> GetLiveStreamDetails(
            IEnumerable<YouTubeSearchItemDto> streams)
        {
            var videoIds = streams.Select(v => v.id.videoId).ToArray();

            var videosResult = await youTubeV3Api.GetVideos(videoIds);

            if (videosResult.IsFailure)
            {
                return Result<Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>, YoutubeErrorType>.Fail(YoutubeErrorType.ProviderNotAvailable);
            }

            return Result<Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>, YoutubeErrorType>.Success(
                videosResult.Value
                    .Map(videos => videos.items.ToDictionary(v => v.id, v => v.liveStreamingDetails))
                    .GetOrElse(new Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>()));
        }

        private async Task<Result<Dictionary<string, YouTubeChannelSnippetDto>, YoutubeErrorType>> GetChannelSnippets(
            IEnumerable<YouTubeSearchItemDto> streams)
        {
            var channelIds = streams.Select(v => v.snippet.channelId).ToArray();

            var channelsResults = await youTubeV3Api.GetChannels(channelIds);

            if (channelsResults.IsFailure)
            {
                return Result<Dictionary<string, YouTubeChannelSnippetDto>, YoutubeErrorType>.Fail(YoutubeErrorType.ProviderNotAvailable);
            }

            return Result<Dictionary<string, YouTubeChannelSnippetDto>, YoutubeErrorType>.Success(
                channelsResults.Value
                    .Map(channels => channels.items.ToDictionary(c => c.id, c => c.snippet))
                    .GetOrElse(new Dictionary<string, YouTubeChannelSnippetDto>()));
        }

        public async Task<GameStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken = null)
        {
            var liveVideosResult = await youTubeV3Api.SearchGamingVideos(filterOptions.GameName, VideoEventType.Live, VideoSortType.ViewCount, pageSize, pageToken);

            if (liveVideosResult.IsFailure || liveVideosResult.Value.IsNothing)
            {
                return GameStreamsDto.Empty;
            }

            var liveVideos = liveVideosResult.Value.Unwrap();

            if (liveVideos.items.Count() == 0)
            {
                return GameStreamsDto.Empty;
            }

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
                Items = mapAsLiveStream(liveVideos.items, channelSnippets.Value, liveStreamDetails.Value),
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
                .Select(channel => new StreamerChannelDto
                {
                    ChannelName = channel.snippet.title,
                    AvatarUrl = channel.snippet.thumbnails.@default.url,
                    ChannelUrl = $"{youTubeBaseUrl}/user/{channel.snippet.title}",
                    Platform = Platform,
                })
                .FirstOrDefault()
            );

            return MaybeResult<StreamerChannelDto, GetStreamerChannelErrorType>.Success(channel);
        }

        public StreamPlatformType Platform => StreamPlatformType.YouTube;
    }
}
