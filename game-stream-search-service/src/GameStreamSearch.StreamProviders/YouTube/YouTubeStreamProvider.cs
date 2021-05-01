using System.Linq;
using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.StreamProviders.YouTube.Gateways.V3;
using GameStreamSearch.StreamProviders.YouTube.Mappers.V3;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.StreamProviders.Const;
using GameStreamSearch.Common;

namespace GameStreamSearch.StreamProviders.YouTube
{
    public class YouTubeStreamProvider : IStreamProvider
    {
        private readonly YouTubeV3Gateway youTubeV3Api;
        private readonly YouTubeStreamMapper streamMapper;
        private readonly YouTubeChannelMapper channelMapper;

        public YouTubeStreamProvider(
            YouTubeV3Gateway youTubeV3Api,
            YouTubeStreamMapper streamMapper,
            YouTubeChannelMapper channelMapper
        )
        {
            this.youTubeV3Api = youTubeV3Api;
            this.streamMapper = streamMapper;
            this.channelMapper = channelMapper;
        }

        public async Task<PlatformStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, PageToken pageToken)
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

                return streamMapper.Map(videos, videoDetailResults, videoChannelResults);
            });

            return streams.GetOrElse(PlatformStreamsDto.Empty(StreamPlatformName));
        }

        public async Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string channelName)
        {
            var channelsResults = await youTubeV3Api.SearchChannelById(channelName, 1);

            return channelMapper.Map(channelsResults);
        }

        public string StreamPlatformName => StreamPlatform.YouTube;
    }
}
