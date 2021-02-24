using System;
using System.Collections.Generic;

namespace GameStreamSearch.Application.Dto.Twitch.Kraken
{
    public class TwitchChannelsDto
    {
        public IEnumerable<TwitchChannelDto> Channels { get; set; }
    }
}
