using System.Threading.Tasks;
using Flurl.Http;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Extensions
{
    public static class FlurlExtensions
    {
        public static async Task<MaybeResult<T, StreamProviderError>> GetOrError<T>(this Task<IFlurlResponse> responseTask)
        {
            var response = await responseTask;

            if (response.StatusCode >= 400)
            {
                return MaybeResult<T, StreamProviderError>.Fail(StreamProviderError.ProviderNotAvailable);
            }

            var payload = await response.GetJsonAsync<T>();

            return MaybeResult<T, StreamProviderError>.Success(payload);
        }
    }
}
