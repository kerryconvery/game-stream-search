using System.Threading.Tasks;
using GameStreamSearch.StreamPlatformApi.DLive.Dto;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamPlatformApi
{
    public enum StreamSortOrder
    {
        New,
        Trending
    };

    public static class DLiveTypeExtensions
    {

        public static string GetAsString(this StreamSortOrder streamSortType)
        {
            return streamSortType.ToString().ToUpper();
        }
    }

    public interface IDLiveApi
    {
        Task<DLiveStreamDto> GetLiveStreams(int pageSize, int pageOffset, StreamSortOrder sortOrder);
        Task<Maybe<DLiveUserDto>> GetUserByDisplayName(string displayName);
    }
}
