using System.Threading.Tasks;
using GameStreamSearch.StreamPlatformApi.Twitch.Dto.Kraken;
using RestSharp;

namespace GameStreamSearch.StreamPlatformApi.Twitch
{
    public class TwitchKrakenApi : ITwitchKrakenApi
    {
        private readonly string twitchApiUrl;
        private readonly string twitchClientId;

        public TwitchKrakenApi(string twitchApiUrl, string twitchClientId)
        {
            this.twitchApiUrl = twitchApiUrl;
            this.twitchClientId = twitchClientId;
        }

        public async Task<TwitchLiveStreamDto> GetLiveStreams(int pageSize, int pageOffset)
        {
            var client = new RestClient(this.twitchApiUrl);

            var request = new RestRequest("/kraken/streams", Method.GET);

            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", twitchClientId);

            request.AddParameter("limit", pageSize);
            request.AddParameter("offset", pageOffset);

            var response = await client.ExecuteAsync<TwitchLiveStreamDto>(request);

            return response.Data;
        }

        public async Task<ProviderApiResult<TwitchChannelsDto>> SearchChannels(string searchTerm, int pageSize, int pageOffset)
        {
            var client = new RestClient(this.twitchApiUrl);

            var request = new RestRequest("/kraken/search/channels", Method.GET);

            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", twitchClientId);

            request.AddParameter("query", searchTerm);
            request.AddParameter("limit", pageSize);
            request.AddParameter("offset", pageOffset);

            var response = await client.ExecuteAsync<TwitchChannelsDto>(request);

            return ProviderApiResult<TwitchChannelsDto>.Success(response.Data);
        }

        public async Task<TwitchLiveStreamDto> SearchStreams(string searchTerm, int pageSize, int pageOffset)
        {
            var client = new RestClient(this.twitchApiUrl);

            var request = new RestRequest("/kraken/search/streams", Method.GET);

            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", twitchClientId);

            request.AddParameter("query", searchTerm);
            request.AddParameter("limit", pageSize);
            request.AddParameter("offset", pageOffset);

            var response = await client.ExecuteAsync<TwitchLiveStreamDto>(request);

            return response.Data;
        }
    }
}
