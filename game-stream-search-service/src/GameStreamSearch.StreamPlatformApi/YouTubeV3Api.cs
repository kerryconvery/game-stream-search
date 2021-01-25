using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Types;
using GameStreamSearch.StreamProviders.Dto.YouTube.YouTubeV3;
using RestSharp;

namespace GameStreamSearch.StreamProviders.Dto
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

        public async Task<MaybeResult<YouTubeSearchDto, YouTubeErrorType>> SearchGamingVideos(string query, VideoEventType eventType, VideoSortType order, int pageSize, string pageToken)
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


            if (response.ResponseStatus == ResponseStatus.Error || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return MaybeResult<YouTubeSearchDto, YouTubeErrorType>.Fail(YouTubeErrorType.ProviderNotAvailable);
            }

            return MaybeResult<YouTubeSearchDto, YouTubeErrorType>.Success(response.Data);
        }

        public async Task<MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>> GetChannels(string[] channelIds)
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

            if (response.ResponseStatus == ResponseStatus.Error || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>.Fail(YouTubeErrorType.ProviderNotAvailable);
            }

            return MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>.Success(response.Data.items);
        }

        public async Task<MaybeResult<IEnumerable<YouTubeVideoDto>, YouTubeErrorType>> GetVideos(string[] videoIds)
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

            if (response.ResponseStatus == ResponseStatus.Error || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return MaybeResult<IEnumerable<YouTubeVideoDto>, YouTubeErrorType>.Fail(YouTubeErrorType.ProviderNotAvailable);
            }

            return MaybeResult<IEnumerable<YouTubeVideoDto>, YouTubeErrorType>.Success(response.Data.items);
        }

        public async Task<MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>> SearchChannelsByUsername(string username, int pageSize)
        {
            var client = new RestClient(this.googleApiUrl);

            var request = new RestRequest("/youtube/v3/channels", Method.GET);

            request.AddParameter("part", "id");
            request.AddParameter("part", "snippet");
            request.AddParameter("forUsername", username);

            request.AddParameter("key", googleApiKey);

            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteAsync<YouTubeChannelsDto>(request);

            if (response.ResponseStatus == ResponseStatus.Error || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>.Fail(YouTubeErrorType.ProviderNotAvailable);
            }

            return MaybeResult<IEnumerable<YouTubeChannelDto>, YouTubeErrorType>.Success(response.Data.items);
        }
    }
}
