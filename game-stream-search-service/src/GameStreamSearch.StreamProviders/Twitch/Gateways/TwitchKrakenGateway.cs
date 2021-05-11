using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using GameStreamSearch.StreamProviders.Extensions;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Twitch.Gateways
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

        public async Task<IEnumerable<TwitchStreamDto>> GetLiveStreams(int pageSize, int pageOffset)
        {
            var response = await BuildPagedRequest("/kraken/streams", pageSize, pageOffset)
                .GetAsync()
                .GetJsonResponseAsync<TwitchLiveStreamDto>();

            return response.streams;
        }

        public async Task<IEnumerable<TwitchStreamDto>> SearchStreams(string searchTerm, int pageSize, int pageOffset)
        {
            var response = await BuildPagedRequest("/kraken/search/streams", pageSize, pageOffset)
                .WithSearchTerm(searchTerm)
                .GetAsync()
                .GetJsonResponseAsync<TwitchLiveStreamDto>();

            return response.streams;
        }

        public async Task<IEnumerable<TwitchChannelDto>> SearchChannels(string searchTerm, int pageSize, int pageOffset)
        {
            var response = await BuildPagedRequest("/kraken/search/channels", pageSize, pageOffset)
                .WithSearchTerm(searchTerm)
                .GetAsync()
                .GetJsonResponseAsync<TwitchChannelsDto>();

            return response.Channels;
        }

        public async Task<Maybe<TwitchChannelDto>> GetChannelByName(string channelName)
        {
            var channels = await SearchChannels(channelName, 1, 0);

            var channel = channels
                .Where(channel => channel.display_name == channelName)
                .FirstOrDefault();

            return Maybe<TwitchChannelDto>.ToMaybe(channel);
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

    public static class TwitchFlurExtensions
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
