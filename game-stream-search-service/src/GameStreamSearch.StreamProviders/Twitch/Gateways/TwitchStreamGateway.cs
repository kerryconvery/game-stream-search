using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;

namespace GameStreamSearch.StreamProviders.Twitch.Gateways
{
    public interface TwitchStreamGateway
    {
        Task<IEnumerable<TwitchStreamDto>> GetLiveStreams(int pageSize, int pageOffset);
        Task<IEnumerable<TwitchStreamDto>> SearchStreams(string searchTerm, int pageSize, int pageOffset);
    }
}
