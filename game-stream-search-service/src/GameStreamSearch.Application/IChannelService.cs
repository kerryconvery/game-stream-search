using System.Threading.Tasks;
using GameStreamSearch.Application.ValueObjects;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Services
{
    public interface IChannelService
    {
        Task<MaybeResult<PlatformChannel, StreamProviderError>> GetStreamerChannel(string streamerName, string streamingPlatformId);
    };
}