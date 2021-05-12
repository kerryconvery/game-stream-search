using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using GameStreamSearch.StreamProviders.DLive.Gateways.Dto;
using GameStreamSearch.StreamProviders.Extensions;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.DLive.Gateways
{
    public class DLiveGraphQLGateway : DLiveStreamGateway, DLiveUserGateway
    {
        private readonly string dliveGraphQLApiUrl;

        public DLiveGraphQLGateway(string dliveGraphQLApiUrl)
        {
            this.dliveGraphQLApiUrl = dliveGraphQLApiUrl;
        }

        public async Task<IEnumerable<DLiveStreamItemDto>> GetLiveStreams(
            int pageSize, int pageOffset, StreamSortOrder sortOrder)
        {
            var graphQuery = new
            {
                query = $"query {{ " +
                    $"livestreams(input: {{ order: {sortOrder.GetAsString()} first: {pageSize} after: \"{pageOffset}\" }}) " +
                    $"{{list {{ title watchingCount thumbnailUrl creator {{ username, displayname, avatar }}}} }} }}",
            };

            var streams = await BuildRequest()
                .PostJsonAsync(graphQuery)
                .GetJsonResponseAsync<DLiveStreamDto>();

            return streams.data.livestreams.list;
        }

        public async Task<Maybe<DLiveUserDto>> GetUserByDisplayName(string displayName)
        {
            var graphQuery = new
            {
                query = $"query {{userByDisplayName(displayname: \"{displayName}\") {{ displayname, avatar }} }}",
            };

            var user = await BuildRequest()
                .PostJsonAsync(graphQuery)
                .GetJsonResponseAsync<DLiveUserByDisplayNameDto>();

            return Maybe<DLiveUserDto>.ToMaybe(user.data.userByDisplayName);
        }

        private IFlurlRequest BuildRequest()
        {
            return dliveGraphQLApiUrl
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Accept", "application/json")
                .AllowAnyHttpStatus();
        }
    }

    public enum StreamSortOrder
    {
        New,
        Trending
    };

    public static class DLiveTypeExtension
    {
        public static string GetAsString(this StreamSortOrder streamSortType)
        {
            return streamSortType.ToString().ToUpper();
        }
    }
}
