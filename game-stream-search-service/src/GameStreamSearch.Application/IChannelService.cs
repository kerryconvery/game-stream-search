using System.Threading.Tasks;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Models
{
    public interface IChannelService
    {
        Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string streamingPlatformName, string streamerName);
    };
}