using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.Dto.DLive;
using GameStreamSearch.Types;
using RestSharp;

namespace GameStreamSearch.StreamProviders.Dto
{
    public class DLiveGraphQLApi : IDLiveApi
    {
        private readonly string dliveGraphQLApiUrl;

        public DLiveGraphQLApi(string dliveGraphQLApiUrl)
        {
            this.dliveGraphQLApiUrl = dliveGraphQLApiUrl;
        }

        public async Task<MaybeResult<DLiveStreamDto, DLiveErrorType>> GetLiveStreams(int pageSize, int pageOffset, StreamSortOrder sortOrder)
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

            if (response.ResponseStatus == ResponseStatus.Error)
            {
                return MaybeResult<DLiveStreamDto, DLiveErrorType>.Fail(DLiveErrorType.ProviderNotAvailable);
            }

            return MaybeResult<DLiveStreamDto, DLiveErrorType>.Success(response.Data);
        }

        public async Task<MaybeResult<DLiveUserDto, DLiveErrorType>> GetUserByDisplayName(string displayName)
        {
            var graphQuery = new
            {
                query = $"query {{userByDisplayName(displayname: \"{displayName}\") {{ displayname, avatar }} }}",
            };

            var client = new RestClient(this.dliveGraphQLApiUrl);

            var request = new RestRequest(Method.POST);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");

            request.AddJsonBody(graphQuery);

            var response = await client.ExecuteAsync<DLiveUserByDisplayNameDto>(request);

            if (response.ResponseStatus == ResponseStatus.Error)
            {
                return MaybeResult<DLiveUserDto, DLiveErrorType>.Fail(DLiveErrorType.ProviderNotAvailable);
            }

            return MaybeResult<DLiveUserDto, DLiveErrorType>.Success(response.Data.data.userByDisplayName);
        }
    }
}
