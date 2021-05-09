using System.Threading.Tasks;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.Common;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.StreamProvider
{
    public interface IStreamProvider
    {
        Task<PlatformStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, PageToken pageToken);
        Task<Maybe<PlatformChannelDto>> GetStreamerChannel(string channelName);
        bool AreFilterOptionsSupported(StreamFilterOptions filterOptions) => true;

        string StreamPlatformName { get; }
    }
}
