using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.ProviderApi.DLive.Dto;

namespace GameStreamSearch.StreamProviders.ProviderApi.DLive.Interfaces
{
    public enum StreamSortOrder
    {
        New,
        Trending
    };

    public static class TypeExtensions
    {

        public static string GetAsString(this StreamSortOrder streamSortType)
        {
            return streamSortType.ToString().ToUpper();
        }
    }

    public interface IDLiveApi
    {
        Task<DLiveStreamDto> GetLiveStreams(int pageSize, int pageOffset, StreamSortOrder sortOrder);
    }
}
