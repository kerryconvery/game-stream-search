using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.Types;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application
{
    public class StreamFilterOptions
    {
        public string GameName { get; set; }
    }

    public enum StreamProviderError
    {
        None,
        ProviderNotAvailable,
    }

    public interface IStreamProvider
    {
        Task<PlatformStreamsDto> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken = null);
        Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string channelName);
        bool AreFilterOptionsSupported(StreamFilterOptions filterOptions) => true;

        StreamPlatform StreamPlatform { get; }
    }
}
