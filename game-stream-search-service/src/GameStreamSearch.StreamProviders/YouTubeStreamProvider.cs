using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Application;
using GameStreamSearch.Types;
using GameStreamSearch.StreamProviders.Gateways;
using GameStreamSearch.StreamProviders.Mappers;

namespace GameStreamSearch.StreamProviders
{
    public class YouTubeStreamProvider : IStreamProvider
    {
        private readonly string streamPlatformId;
        private readonly YouTubeV3Gateway youTubeV3Api;
        private readonly YouTubeStreamMapper streamMapper;
        private readonly YouTubeChannelMapper channelMapper;

        public YouTubeStreamProvider(
            string streamPlatformId,
            YouTubeV3Gateway youTubeV3Api,
            YouTubeStreamMapper streamMapper,
            YouTubeChannelMapper channelMapper
        )
        {
            this.streamPlatformId = streamPlatformId;
            this.youTubeV3Api = youTubeV3Api;
            this.streamMapper = streamMapper;
            this.channelMapper = channelMapper;
        }

        public async Task<PlatformStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken)
        {
            var liveVideosResult = await youTubeV3Api.SearchGamingVideos(
                filterOptions.GameName, VideoEventType.Live, VideoSortType.ViewCount, pageSize, pageToken);

            var streams = await liveVideosResult.ChainAsync(async videos =>
            {
                var videoIds = videos.items.Select(v => v.id.videoId);
                var channelIds = videos.items.Select(v => v.snippet.channelId);

                var getVideoDetails = youTubeV3Api.GetVideos(videoIds.ToArray());
                var getVideoChannels = youTubeV3Api.GetChannels(channelIds.ToArray());

                var videoDetailResults = await getVideoDetails;
                var videoChannelResults = await getVideoChannels;

                return streamMapper.Map(StreamPlatformId, videos, videoDetailResults, videoChannelResults);
            });

            return streams.GetOrElse(PlatformStreamsDto.Empty(StreamPlatformId));
        }

        public async Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string channelName)
        {
            var channelsResults = await youTubeV3Api.SearchChannelsByUsername(channelName, 1);

            return channelMapper.Map(StreamPlatformId, channelsResults);
        }

        public string StreamPlatformId => streamPlatformId;
    }
}
