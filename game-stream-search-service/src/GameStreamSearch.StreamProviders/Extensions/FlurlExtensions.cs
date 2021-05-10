using System.Threading.Tasks;
using Flurl.Http;

namespace GameStreamSearch.StreamProviders.Extensions
{
    public static class FlurlExtensions
    {
        public static async Task<T> GetJsonResponseAsync<T>(this Task<IFlurlResponse> responseTask)
        {
            var response = await responseTask;

            return await response.GetJsonAsync<T>();
        }
    }
}
