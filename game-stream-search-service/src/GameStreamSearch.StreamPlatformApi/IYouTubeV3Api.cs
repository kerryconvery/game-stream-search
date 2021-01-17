using System;
using System.Threading.Tasks;
using GameStreamSearch.StreamPlatformApi.YouTube.Dto.YouTubeV3;

namespace GameStreamSearch.StreamPlatformApi
{
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
            switch(videoSortType)
            {
                case VideoSortType.VideoCount: return "videoCount";
                case VideoSortType.ViewCount: return "viewCount";
                default: return videoSortType.ToString().ToLower();
            }
        }
    }

    public interface IYouTubeV3Api
    {
        Task<YouTubeSearchDto> SearchGamingVideos(string query, VideoEventType eventType, VideoSortType order, int pageSize, string pageToken);
        Task<YouTubeChannelsDto> GetChannels(string[] channelIds);
        Task<YouTubeVideosDto> GetVideos(string[] videoIds);
        Task<ProviderApiResult<YouTubeChannelsDto>> SearchChannelsByUsername(string username, int pageSize);
    }
}
