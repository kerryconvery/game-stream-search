using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Dto.YouTubeV3;
using GameStreamSearch.StreamProviders.ProviderApi.YouTube.Interfaces;
using RestSharp;

namespace GameStreamSearch.StreamProviders.ProviderApi.YouTube
{
    public class YouTubeV3Api : IYouTubeV3Api
    {
        private readonly string googleApiUrl;
        private readonly string googleApiKey;

        public YouTubeV3Api(string googleApiUrl, string googleApiKey)
        {
            this.googleApiUrl = googleApiUrl;
            this.googleApiKey = googleApiKey;
        }

        public async Task<YouTubeSearchDto> SearchGamingVideos(string query, VideoEventType eventType, VideoSortType order, int pageSize, string pageToken)
        {
            var client = new RestClient(this.googleApiUrl);

            var request = new RestRequest("/youtube/v3/search", Method.GET);

            request.AddParameter("part", "snippet");
            request.AddParameter("eventType", eventType.GetAsString());
            request.AddParameter("q", query);
            request.AddParameter("type", "video");
            request.AddParameter("videoCategoryId", 20);
            request.AddParameter("key", googleApiKey);
            request.AddParameter("maxResults", pageSize);
            request.AddParameter("pageToken", pageToken);
            request.AddParameter("order", order.GetAsString());

            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync<YouTubeSearchDto>(request);

            return response.Data;
        }

        public async Task<YouTubeChannelsDto> GetChannels(string[] channelIds)
        {
            var client = new RestClient(this.googleApiUrl);

            var request = new RestRequest("/youtube/v3/channels", Method.GET);


            request.AddParameter("part", "id");
            request.AddParameter("part", "snippet");

            foreach (var id in channelIds)
            {
                request.AddParameter("id", id);
            }

            request.AddParameter("key", googleApiKey);

            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync<YouTubeChannelsDto>(request);

            return response.Data;
        }

        public async Task<YouTubeVideosDto> GetVideos(string[] videoIds)
        {
            var client = new RestClient(this.googleApiUrl);

            var request = new RestRequest("/youtube/v3/videos", Method.GET);

            request.AddParameter("part", "id");
            request.AddParameter("part", "statistics");
            request.AddParameter("part", "liveStreamingDetails");

            foreach (var id in videoIds)
            {
                request.AddParameter("id", id);
            }

            request.AddParameter("key", googleApiKey);

            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync<YouTubeVideosDto>(request);

            return response.Data;
        }
    }
}
