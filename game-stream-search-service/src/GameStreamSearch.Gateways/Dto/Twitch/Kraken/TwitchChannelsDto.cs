using System;
using System.Collections.Generic;

namespace GameStreamSearch.Gateways.Dto.Twitch.Kraken
{
    public class TwitchChannelsDto
    {
        public IEnumerable<TwitchChannelDto> Channels { get; set; }
    }
}
