using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Models
{
    public interface IChannelService
    {
        Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string streamingPlatformName, string streamerName);
    };
}