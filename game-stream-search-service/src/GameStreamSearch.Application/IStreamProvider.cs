using System;
using System.Threading.Tasks;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Application.Enums;
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
        Task<PlatformStreams> GetLiveStreams(StreamFilterOptions filterOptions, int pageSize, string pageToken = null);
        Task<MaybeResult<PlatformChannel, StreamProviderError>> GetStreamerChannel(string channelName);
        bool AreFilterOptionsSupported(StreamFilterOptions filterOptions) => true;

        StreamPlatform StreamPlatform { get; }
    }
}
