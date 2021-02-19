using System.Threading.Tasks;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Services
{
    public interface IChannelService
    {
        Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string streamerName, string streamingPlatformId);
    };
}