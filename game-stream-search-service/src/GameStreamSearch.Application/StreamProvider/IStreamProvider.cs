using System.Threading.Tasks;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.Domain;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.StreamProvider
{
    public interface IStreamProvider
    {
        Task<PlatformStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, PageToken pageToken);
        Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string channelName);
        bool AreFilterOptionsSupported(StreamFilterOptions filterOptions) => true;

        StreamPlatform StreamPlatform { get; }
    }
}
