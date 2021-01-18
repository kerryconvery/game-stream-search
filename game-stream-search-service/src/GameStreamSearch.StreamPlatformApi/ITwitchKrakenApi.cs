using System.Threading.Tasks;
using GameStreamSearch.StreamPlatformApi.Twitch.Dto.Kraken;

namespace GameStreamSearch.StreamPlatformApi
{
    public interface ITwitchKrakenApi
    {
        Task<TwitchLiveStreamDto> SearchStreams(string searchTerm, int pageSize, int pageOffset);
        Task<TwitchLiveStreamDto> GetLiveStreams(int pageSize, int pageOffset);
        Task<TwitchChannelsDto> SearchChannels(string searchTerm, int pageSize, int pageOffset);
    }
}
