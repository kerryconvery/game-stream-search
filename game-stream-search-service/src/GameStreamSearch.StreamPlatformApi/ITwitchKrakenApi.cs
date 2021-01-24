using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.StreamPlatformApi.Twitch.Dto.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamPlatformApi
{
    public enum TwitchErrorType
    {
        None,
        ProviderNotAvailable,
    }

    public interface ITwitchKrakenApi
    {
        Task<MaybeResult<IEnumerable<TwitchStreamDto>, TwitchErrorType>> SearchStreams(string searchTerm, int pageSize, int pageOffset);
        Task<MaybeResult<IEnumerable<TwitchStreamDto>, TwitchErrorType>> GetLiveStreams(int pageSize, int pageOffset);
        Task<MaybeResult<TwitchChannelsDto, TwitchErrorType>> SearchChannels(string searchTerm, int pageSize, int pageOffset);
    }
}
