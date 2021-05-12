using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Twitch.Gateways
{
    public interface TwitchChannelGateway
    {
        Task<Maybe<TwitchChannelDto>> GetChannelByName(string channelName);
    }
}
