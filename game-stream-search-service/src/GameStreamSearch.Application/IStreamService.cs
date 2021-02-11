using System.Threading.Tasks;
using GameStreamSearch.Application.ValueObjects;

namespace GameStreamSearch.Application.Services
{
    public interface IStreamService
    {
        Task<Streams> GetStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken);
    };
}