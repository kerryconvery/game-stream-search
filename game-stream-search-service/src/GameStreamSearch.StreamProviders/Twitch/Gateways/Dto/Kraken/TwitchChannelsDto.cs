using System;
using System.Collections.Generic;

namespace GameStreamSearch.StreamProviders.Twitch.Gateways.Dto.Kraken
{
    public class TwitchChannelsDto
    {
        public IEnumerable<TwitchChannelDto> Channels { get; set; }
    }
}
