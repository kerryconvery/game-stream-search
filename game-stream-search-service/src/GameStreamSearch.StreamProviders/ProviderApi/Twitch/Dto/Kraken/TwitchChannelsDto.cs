using System;
using System.Collections.Generic;

namespace GameStreamSearch.StreamProviders.ProviderApi.Twitch.Dto.Kraken
{
    public class TwitchChannelsDto
    {
        public IEnumerable<TwitchChannelDto> Channels { get; set; }
    }
}
