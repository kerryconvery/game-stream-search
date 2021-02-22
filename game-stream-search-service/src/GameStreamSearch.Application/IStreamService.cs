using System.Threading.Tasks;
using GameStreamSearch.Application.Types;

namespace GameStreamSearch.Application.Types
{
    public interface IStreamService
    {
        Task<PlatformStreamsDto> GetStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken);
    };
}