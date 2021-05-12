using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using System.Linq;
using GameStreamSearch.StreamProviders.Extensions;
using GameStreamSearch.StreamProviders.YouTube.Gateways.Dto.V3;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.YouTube.Gateways.V3
{
    public class YouTubeV3Gateway : YouTubeChannelGateway
    {
        private readonly string googleApiUrl;
        private readonly string googleApiKey;

        public YouTubeV3Gateway(string googleApiUrl, string googleApiKey)
        {
            this.googleApiUrl = googleApiUrl;
            this.googleApiKey = googleApiKey;
        }

        public Task<YouTubeSearchDto> SearchGamingVideos(
            string query, VideoEventType eventType, VideoSortType order, int pageSize, string pageToken)
        {
            return BuildRequest("/youtube/v3/search")
                .SetQueryParam("part", "snippet")
                .SetQueryParam("eventType", eventType.GetAsString())
                .SetQueryParam("q", query)
                .SetQueryParam("type", "video")
                .SetQueryParam("videoCategoryId", 20)
                .SetQueryParam("maxResults", pageSize)
                .SetQueryParam("pageToken", pageToken)
                .SetQueryParam("order", order.GetAsString())
                .GetAsync()
                .GetJsonResponseAsync<YouTubeSearchDto>();
        }

        public async Task<IEnumerable<YouTubeVideoDto>> GetVideos(string[] videoIds)
        {
            var response = await BuildRequest("/youtube/v3/videos")
                .SetQueryParam("part", "id")
                .SetQueryParam("part", "statistics")
                .SetQueryParam("part", "liveStreamingDetails")
                .SetQueryParams(videoIds.Select(id => $"id={id}").ToArray())
                .GetAsync()
                .GetJsonResponseAsync<YouTubeVideosDto>();

            return response.items;
        }

        private IFlurlRequest BuildRequest(string endpoint)
        {
            return googleApiUrl
                .AppendPathSegment(endpoint)
                .WithHeader("Accept", "application/json")
                .SetQueryParam("key", googleApiKey)
                .AllowAnyHttpStatus();
        }

        public async Task<Maybe<YouTubeChannelDto>> GetChannelByName(string channelName)
        {
            var channels = await SearchChannelById(channelName, 1);
            var channel = channels.FirstOrDefault();

            return Maybe<YouTubeChannelDto>.ToMaybe(channel);
        }

        public async Task<IEnumerable<YouTubeChannelDto>> SearchChannelById(string channelId, int pageSize)
        {
            var response = await BuildRequest("/youtube/v3/channels")
                .SetQueryParam("part", "id")
                .SetQueryParam("part", "snippet")
                .SetQueryParam("id", channelId)
                .SetQueryParam("maxResults", pageSize)
                .GetAsync()
                .GetJsonResponseAsync<YouTubeChannelsDto>();

            return response.items;
        }

        public async Task<IEnumerable<YouTubeChannelDto>> BulkGetAvartarByChannelId(string[] channelIds)
        {
            var response = await BuildRequest("/youtube/v3/channels")
                .SetQueryParam("part", "id")
                .SetQueryParam("part", "snippet")
                .SetQueryParams(channelIds.Select(id => $"id={id}").ToArray())
                .GetAsync()
                .GetJsonResponseAsync<YouTubeChannelsDto>();

            return response.items;

        }
    }

    public enum VideoEventType
    {
        Completed,
        Live,
        Upcoming
    }

    public enum VideoSortType
    {
        Date,
        Rating,
        Relevance,
        Title,
        VideoCount,
        ViewCount
    }

    public static class YouTubeTypeExtensions
    {
        public static string GetAsString(this VideoEventType videoEventType)
        {
            return videoEventType.ToString().ToLower();
        }

        public static string GetAsString(this VideoSortType videoSortType)
        {
            switch (videoSortType)
            {
                case VideoSortType.VideoCount: return "videoCount";
                case VideoSortType.ViewCount: return "viewCount";
                default: return videoSortType.ToString().ToLower();
            }
        }
    }
}
