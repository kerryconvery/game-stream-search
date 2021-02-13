using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using GameStreamSearch.Application;
using GameStreamSearch.StreamProviders.Dto.Twitch.Kraken;
using GameStreamSearch.StreamProviders.Extensions;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Gateways
{
    public class TwitchKrakenGateway
    {
        private readonly string twitchApiUrl;
        private readonly string twitchClientId;

        public TwitchKrakenGateway(string twitchApiUrl, string twitchClientId)
        {
            this.twitchApiUrl = twitchApiUrl;
            this.twitchClientId = twitchClientId;
        }

        public async Task<MaybeResult<IEnumerable<TwitchStreamDto>, StreamProviderError>> GetLiveStreams(int pageSize, int pageOffset)
        {
            var streams = await BuildPagedRequest("/kraken/streams", pageSize, pageOffset)
                .GetAsync()
                .GetOrError<TwitchLiveStreamDto>();

            return streams.Select(s => s.streams);
        }

        public async Task<MaybeResult<IEnumerable<TwitchChannelDto>, StreamProviderError>> SearchChannels(string searchTerm, int pageSize, int pageOffset)
        {
            var response = await BuildPagedRequest("/kraken/search/channels", pageSize, pageOffset)
                .WithSearchTerm(searchTerm)
                .GetAsync()
                .GetOrError<TwitchChannelsDto>();

            return response.Select(c => c.Channels);
        }

        public async Task<MaybeResult<IEnumerable<TwitchStreamDto>, StreamProviderError>> SearchStreams(string searchTerm, int pageSize, int pageOffset)
        {
            var streams = await BuildPagedRequest("/kraken/search/streams", pageSize, pageOffset)
                .WithSearchTerm(searchTerm)
                .GetAsync()
                .GetOrError<TwitchLiveStreamDto>();

            return streams.Select(s => s.streams);
        }

        private IFlurlRequest BuildPagedRequest(string endpoint, int pageSize, int pageOffset)
        {
            return twitchApiUrl
                .AppendPathSegment(endpoint)
                .WithHeader("Client-ID", twitchClientId)
                .WithHeader("Accept", "application/vnd.twitchtv.v5+json")
                .WithPaging(pageSize, pageOffset);
        }
    }

    public static class Extensions
    {
        public static IFlurlRequest WithSearchTerm(this IFlurlRequest request, string searchTerm)
        {
            return request.SetQueryParam("query", searchTerm);
        }

        public static IFlurlRequest WithPaging(this IFlurlRequest request, int pageSize, int pageOffset)
        {
            return request
                .SetQueryParam("limit", pageSize)
                .SetQueryParam("offset", pageOffset)
                .AllowAnyHttpStatus();
        }
    }

}
