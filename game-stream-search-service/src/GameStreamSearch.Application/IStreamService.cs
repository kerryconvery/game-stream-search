using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Models;

namespace GameStreamSearch.Application
{
    public class StreamFilterOptions
    {
        public string GameName { get; set; }
    }

    public interface IStreamService
    {
        IEnumerable<string> GetSupportingPlatforms(StreamFilterOptions streamFilterOptions);
        Task<IEnumerable<PlatformStreamsDto>> GetStreams(
            IEnumerable<string> streamPlatforms, StreamFilterOptions filterOptions, int pageSize, PageTokens pageTokens);
    };
}