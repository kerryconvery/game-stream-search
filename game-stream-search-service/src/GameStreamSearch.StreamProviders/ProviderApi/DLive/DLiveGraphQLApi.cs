using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.ProviderApi.DLive.Dto;
using GameStreamSearch.StreamProviders.ProviderApi.DLive.Interfaces;
using RestSharp;

namespace GameStreamSearch.StreamProviders.ProviderApi.DLive
{
    public class DLiveGraphQLApi : IDLiveApi
    {
        private readonly string dliveGraphQLApiUrl;

        public DLiveGraphQLApi(string dliveGraphQLApiUrl)
        {;
            this.dliveGraphQLApiUrl = dliveGraphQLApiUrl;
        }

        public async Task<DLiveStreamDto> GetLiveStreams(int pageSize, int pageOffset, StreamSortOrder sortOrder)
        {
            var graphQuery = new
            {
                query = $"query {{ " +
                    $"livestreams(input: {{ order: {sortOrder.GetAsString()} first: {pageSize} after: \"{pageOffset}\" }}) " +
                    $"{{list {{ title watchingCount thumbnailUrl creator {{ username, displayname, avatar }}}} }} }}",
            };

            var client = new RestClient(this.dliveGraphQLApiUrl);

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");

            request.AddJsonBody(graphQuery);

            var response = await client.ExecuteAsync<DLiveStreamDto>(request);

            return response.Data;
        }
    }
}
