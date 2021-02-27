using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.StreamProvider;
using GameStreamSearch.Application.StreamProvider.Dto;

namespace GameStreamSearch.Application.Services.StreamProvider
{
    public interface IStreamService
    {
        IEnumerable<string> GetSupportingPlatforms(StreamFilterOptions streamFilterOptions);
        Task<IEnumerable<PlatformStreamsDto>> GetStreams(
            IEnumerable<string> streamPlatforms, StreamFilterOptions filterOptions, int pageSize, PageTokens pageTokens);
    };
}