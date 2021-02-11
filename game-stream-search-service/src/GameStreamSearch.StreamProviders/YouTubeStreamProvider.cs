using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.StreamProviders.Dto.YouTube.YouTubeV3;
using GameStreamSearch.Application;
using GameStreamSearch.Types;
using GameStreamSearch.StreamProviders.Gateways;
using GameStreamSearch.StreamProviders.Mappers;

namespace GameStreamSearch.StreamProviders
{
    public class YouTubeStreamProvider : IStreamProvider
    {
        private readonly YouTubeV3Gateway youTubeV3Api;
        private readonly YouTubeMapper youTubeMapper;

        public YouTubeStreamProvider(YouTubeV3Gateway youTubeV3Api, YouTubeMapper youTubeMapper)
        {
            this.youTubeV3Api = youTubeV3Api;
            this.youTubeMapper = youTubeMapper;
        }

        public async Task<Streams> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            var liveVideosResult = await youTubeV3Api.SearchGamingVideos(
                filterOptions.GameName, VideoEventType.Live, VideoSortType.ViewCount, pageSize, pageToken);

            var streams = await liveVideosResult.ChainAsync(async videos =>
            {
                var videoIds = videos.items.Select(v => v.id.videoId);
                var channelIds = videos.items.Select(v => v.snippet.channelId);

                var getVideoDetails = GetVideoDetails(videoIds.ToArray());
                var getVideoChannels = GetVideoChannels(channelIds.ToArray());

                var videoDetailResults = await getVideoDetails;
                var videoChannelResults = await getVideoChannels;

                return AssembleStreams(videos, videoDetailResults, videoChannelResults);
            });

            return streams.GetOrElse(Streams.Empty);

        }

        private async Task<MaybeResult<Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>, StreamProviderError>> GetVideoDetails(
            string[] videoIds)
        {
            var videosResult = await youTubeV3Api.GetVideos(videoIds);

            return videosResult
                .Select(videos => videos.ToDictionary(v => v.id, v => v.liveStreamingDetails));
        }

        private async Task<MaybeResult<Dictionary<string, YouTubeChannelSnippetDto>, StreamProviderError>> GetVideoChannels(
            string[] channelIds)
        {
            var channelsResults = await youTubeV3Api.GetChannels(channelIds);

            return channelsResults
                    .Select(channels => channels.ToDictionary(c => c.id, c => c.snippet));
        }

        private MaybeResult<Streams, StreamProviderError> AssembleStreams(
            YouTubeSearchDto videoSearchResults,
            MaybeResult<Dictionary<string, YouTubeVideoLiveStreamingDetailsDto>, StreamProviderError> videoDetailResults,
            MaybeResult<Dictionary<string, YouTubeChannelSnippetDto>, StreamProviderError> videoChannelResults)
        {
            return videoDetailResults.Chain(videoDetails =>
            {
                return videoChannelResults.Select(videoChannels =>
                {
                    return youTubeMapper.ToPlatformStreams(videoSearchResults, videoChannels, videoDetails);
                });
            });
        }

        public async Task<MaybeResult<PlatformChannel, StreamProviderError>> GetStreamerChannel(string channelName)
        {
            var channelsResult = await youTubeV3Api.SearchChannelsByUsername(channelName, 1);

            return channelsResult.Select(channels => channels
                .Select(channel => youTubeMapper.ToPlatformChannel(channel.snippet))
                .FirstOrDefault());
        }

        public StreamPlatformType Platform => StreamPlatformType.YouTube;
    }
}
