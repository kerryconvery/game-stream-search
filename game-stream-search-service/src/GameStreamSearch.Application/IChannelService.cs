using System.Threading.Tasks;
using GameStreamSearch.Application.Types;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Types
{
    public interface IChannelService
    {
        Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string streamerName, string streamingPlatformId);
    };
}