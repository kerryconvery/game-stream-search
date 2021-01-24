using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.StreamPlatformApi.Twitch.Dto.Kraken;
using GameStreamSearch.Types;
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

        public async Task<MaybeResult<IEnumerable<TwitchStreamDto>, TwitchErrorType>> GetLiveStreams(int pageSize, int pageOffset)
        {
            var client = new RestClient(this.twitchApiUrl);

            var request = new RestRequest("/kraken/streams", Method.GET);

            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", twitchClientId);

            request.AddParameter("limit", pageSize);
            request.AddParameter("offset", pageOffset);

            var response = await client.ExecuteAsync<TwitchLiveStreamDto>(request);

            if (response.ResponseStatus == ResponseStatus.Error)
            {
                return MaybeResult<IEnumerable<TwitchStreamDto>, TwitchErrorType>.Fail(TwitchErrorType.ProviderNotAvailable);
            }

            return MaybeResult<IEnumerable<TwitchStreamDto>, TwitchErrorType>.Success(response.Data.streams);
        }

        public async Task<MaybeResult<TwitchChannelsDto, TwitchErrorType>> SearchChannels(string searchTerm, int pageSize, int pageOffset)
        {
            var client = new RestClient(this.twitchApiUrl);

            var request = new RestRequest("/kraken/search/channels", Method.GET);

            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", twitchClientId);

            request.AddParameter("query", searchTerm);
            request.AddParameter("limit", pageSize);
            request.AddParameter("offset", pageOffset);

            var response = await client.ExecuteAsync<TwitchChannelsDto>(request);

            if (response.ResponseStatus == ResponseStatus.Error)
            {
                return MaybeResult<TwitchChannelsDto, TwitchErrorType>.Fail(TwitchErrorType.ProviderNotAvailable);
            }

            return MaybeResult<TwitchChannelsDto, TwitchErrorType>.Success(response.Data);
        }

        public async Task<MaybeResult<IEnumerable<TwitchStreamDto>, TwitchErrorType>> SearchStreams(string searchTerm, int pageSize, int pageOffset)
        {
            var client = new RestClient(this.twitchApiUrl);

            var request = new RestRequest("/kraken/search/streams", Method.GET);

            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", twitchClientId);

            request.AddParameter("query", searchTerm);
            request.AddParameter("limit", pageSize);
            request.AddParameter("offset", pageOffset);

            var response = await client.ExecuteAsync<TwitchLiveStreamDto>(request);

            if (response.ResponseStatus == ResponseStatus.Error)
            {
                return MaybeResult<IEnumerable<TwitchStreamDto>, TwitchErrorType>.Fail(TwitchErrorType.ProviderNotAvailable);
            }

            return MaybeResult<IEnumerable<TwitchStreamDto>, TwitchErrorType>.Success(response.Data.streams);
        }
    }
}
