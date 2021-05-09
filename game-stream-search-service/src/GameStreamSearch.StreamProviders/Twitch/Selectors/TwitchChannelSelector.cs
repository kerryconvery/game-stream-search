using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.StreamProviders.Twitch.Selectors
{
    public static class TwitchChannelSelector
    {
        public static Maybe<TwitchChannelDto> Select(string channelName, IEnumerable<TwitchChannelDto> channels)
        {
            return Maybe< TwitchChannelDto>.ToMaybe(channels
                    .Where(channel => channel.display_name.Equals(channelName, StringComparison.CurrentCultureIgnoreCase))
                    .FirstOrDefault());
        }
    }
}
