using System.Threading.Tasks;
using GameStreamSearch.Application.Models;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Models
{
    public interface IChannelService
    {
        Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string streamerName, string streamingPlatformId);
    };
}