using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using GameStreamSearch.Application.Dto.DLive;
using GameStreamSearch.Application.Extensions;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application
{
    public class DLiveGraphQLGateway
    {
        private readonly string dliveGraphQLApiUrl;

        public DLiveGraphQLGateway(string dliveGraphQLApiUrl)
        {
            this.dliveGraphQLApiUrl = dliveGraphQLApiUrl;
        }

        public async Task<MaybeResult<IEnumerable<DLiveStreamItemDto>, StreamProviderError>> GetLiveStreams(
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
                .GetOrError<DLiveStreamDto>();

            return streams.Select(s => s.data.livestreams.list);
        }

        public async Task<MaybeResult<DLiveUserDto, StreamProviderError>> GetUserByDisplayName(string displayName)
        {
            var graphQuery = new
            {
                query = $"query {{userByDisplayName(displayname: \"{displayName}\") {{ displayname, avatar }} }}",
            };

            var streams = await BuildRequest()
                .PostJsonAsync(graphQuery)
                .GetOrError<DLiveUserByDisplayNameDto>();

            return streams.Select(s => s.data.userByDisplayName);
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
