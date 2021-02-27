using System.Threading.Tasks;
using GameStreamSearch.Application.StreamProvider.Dto;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Services.StreamProvider
{
    public interface IChannelService
    {
        Task<MaybeResult<PlatformChannelDto, StreamProviderError>> GetStreamerChannel(string streamingPlatformName, string streamerName);
    };
}