using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.ProviderApi.Twitch.Dto.Kraken;

namespace GameStreamSearch.StreamProviders.ProviderApi.Twitch.Interfaces
{
    public interface ITwitchKrakenApi
    {
        Task<TwitchLiveStreamDto> SearchStreams(string searchTerm, int pageSize, int pageOffset);
        Task<TwitchLiveStreamDto> GetLiveStreams(int pageSize, int pageOffset);
        Task<TwitchChannelsDto> SearchChannels(string searchTerm, int pageSize, int pageOffset);
    }
}
