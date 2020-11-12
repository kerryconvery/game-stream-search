using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Interfaces;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Dto.YouTubeV3;
using GameStreamSearch.StreamProviders.Builders;
using GameStreamSearch.Application.Exceptions;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.StreamProviders
{
    public class YouTubeStreamProvider : IStreamProvider
    {
        private readonly IYouTubeWatchUrlBuilder urlBuilder;
        private readonly IYouTubeV3Api youTubeV3Api;

        public YouTubeStreamProvider(IYouTubeWatchUrlBuilder urlBuilder, IYouTubeV3Api youTubeV3Api)
        {
            this.urlBuilder = urlBuilder;
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
                    StreamUrl = urlBuilder.Build(v.id.videoId),
                    StreamPlatformName = Platform.GetFriendlyName(),
                    IsLive = true,
                    Views = streamDetails != null ? streamDetails.concurrentViewers : 0,
                };
            });

            return gameStreams;
        }

        private async Task<Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>> GetLiveStreamDetails(
            IEnumerable<YouTubeSearchItemDto> streams)
        {
            var videoIds = streams.Select(v => v.id.videoId).ToArray();

            var videos = await youTubeV3Api.GetVideos(videoIds);

            return videos.items.ToDictionary(v => v.id, v => v.liveStreamingDetails);
        }

        private async Task<Dictionary<string, YouTubeChannelSnippetDto>> GetChannelSnippets(
            IEnumerable<YouTubeSearchItemDto> streams)
        {
            var channelIds = streams.Select(v => v.snippet.channelId).ToArray();

            var channels = await youTubeV3Api.GetChannels(channelIds);

            return channels.items.ToDictionary(c => c.id, c => c.snippet);
        }

        public async Task<GameStreamsDto> GetLiveStreams(StreamFilterOptionsDto filterOptions, int pageSize, string pageToken = null)
        {
            var liveVideos = await youTubeV3Api.SearchGamingVideos(filterOptions.GameName, VideoEventType.Live, VideoSortType.ViewCount, pageSize, pageToken);

            if (liveVideos.items == null || liveVideos.items.Count() == 0)
            {
                return GameStreamsDto.Empty();
            }

            var getLiveStreamDetailsTask = GetLiveStreamDetails(liveVideos.items);
            var getChannelSnippetsTask = GetChannelSnippets(liveVideos.items);

            var liveStreamDetails = await getLiveStreamDetailsTask;
            var channelSnippets = await getChannelSnippetsTask;

            return new GameStreamsDto
            {
                Items = mapAsLiveStream(liveVideos.items, channelSnippets, liveStreamDetails),
                NextPageToken = liveVideos.nextPageToken,
            };
        }

        public async Task<StreamerChannelDto> GetStreamerChannel(string channelName)
        {
            var channels = await youTubeV3Api.SearchChannelsByUsername(channelName, 1);

            if (channels.items == null)
            {
                throw new StreamProviderUnavailableException();
            }

            if (channels.items.Count() == 0)
            {
                return null;
            }

            if (!channels.items.First().snippet.title.Equals(channelName, System.StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }

            return new StreamerChannelDto
            {
                ChannelName = channels.items.First().snippet.title,
                AvatarUrl = channels.items.First().snippet.thumbnails.@default.url,
                Platform = Platform,
            };
        }

        public StreamingPlatform Platform => StreamingPlatform.youtube;
    }
}
