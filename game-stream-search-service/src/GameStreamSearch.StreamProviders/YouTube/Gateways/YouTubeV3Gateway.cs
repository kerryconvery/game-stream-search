using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using System.Linq;
using GameStreamSearch.Types;
using GameStreamSearch.StreamProviders.Extensions;
using GameStreamSearch.StreamProviders.YouTube.Gateways.Dto.V3;

namespace GameStreamSearch.StreamProviders.YouTube.Gateways.V3
{
    public class YouTubeV3Gateway
    {
        private readonly string googleApiUrl;
        private readonly string googleApiKey;

        public YouTubeV3Gateway(string googleApiUrl, string googleApiKey)
        {
            this.googleApiUrl = googleApiUrl;
            this.googleApiKey = googleApiKey;
        }

        public async Task<MaybeResult<YouTubeSearchDto, StreamProviderError>> SearchGamingVideos(
            string query, VideoEventType eventType, VideoSortType order, int pageSize, string pageToken)
        {
            return await BuildRequest("/youtube/v3/search")
                .SetQueryParam("part", "snippet")
                .SetQueryParam("eventType", eventType.GetAsString())
                .SetQueryParam("q", query)
                .SetQueryParam("type", "video")
                .SetQueryParam("videoCategoryId", 20)
                .SetQueryParam("maxResults", pageSize)
                .SetQueryParam("pageToken", pageToken)
                .SetQueryParam("order", order.GetAsString())
                .GetAsync()
                .GetOrError<YouTubeSearchDto>();
        }

        public async Task<MaybeResult<IEnumerable<YouTubeChannelDto>, StreamProviderError>> GetChannels(string[] channelIds)
        {
            var response = await BuildRequest("/youtube/v3/channels")
                .SetQueryParam("part", "id")
                .SetQueryParam("part", "snippet")
                .SetQueryParams(channelIds.Select(id => $"id={id}").ToArray())
                .GetAsync()
                .GetOrError<YouTubeChannelsDto>();

            return response.Select(c => c.items);
        }

        public async Task<MaybeResult<IEnumerable<YouTubeVideoDto>, StreamProviderError>> GetVideos(string[] videoIds)
        {
            var response = await BuildRequest("/youtube/v3/videos")
                .SetQueryParam("part", "id")
                .SetQueryParam("part", "statistics")
                .SetQueryParam("part", "liveStreamingDetails")
                .SetQueryParams(videoIds.Select(id => $"id={id}").ToArray())
                .GetAsync()
                .GetOrError<YouTubeVideosDto>();

            return response.Select(v => v.items);
        }

        public async Task<MaybeResult<IEnumerable<YouTubeChannelDto>, StreamProviderError>> SearchChannelsByUsername(string username, int pageSize)
        {
            var response = await BuildRequest("/youtube/v3/channels")
                .SetQueryParam("part", "id")
                .SetQueryParam("part", "snippet")
                .SetQueryParam("forUsername", username)
                .SetQueryParam("maxResults", pageSize)
                .GetAsync()
                .GetOrError<YouTubeChannelsDto>();

            return response.Select(v => v.items);
        }

        private IFlurlRequest BuildRequest(string endpoint)
        {
            return googleApiUrl
                .AppendPathSegment(endpoint)
                .WithHeader("Accept", "application/json")
                .SetQueryParam("key", googleApiKey)
                .AllowAnyHttpStatus();
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
