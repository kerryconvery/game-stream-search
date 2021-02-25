﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameStreamSearch.Gateways.Dto.Twitch.Kraken;
using GameStreamSearch.Types;

namespace GameStreamSearch.Application.Selectors
{
    public static class TwitchChannelSelector
    {
        public static MaybeResult<TwitchChannelDto, StreamProviderError> Select(
            string channelName, MaybeResult<IEnumerable<TwitchChannelDto>, StreamProviderError> channelSearchResults)
        {
            return channelSearchResults
                .Select(channels => channels
                    .Where(channel => channel.display_name.Equals(channelName, StringComparison.CurrentCultureIgnoreCase))
                    .FirstOrDefault());
        }
    }
}
