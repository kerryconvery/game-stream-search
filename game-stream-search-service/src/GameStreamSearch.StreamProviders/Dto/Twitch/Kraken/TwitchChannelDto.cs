using System;
using GameStreamSearch.Application.Dto;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.StreamProviders.Dto.Twitch.Kraken
{
    public class TwitchChannelDto
    {
        public string game { get; set; }
        public string display_name { get; set; }
        public string logo { get; set; }
        public string url { get; set; }
        public string status { get; set; }

        public StreamerChannelDto ToStreamerChannelDto()
        {
            return new StreamerChannelDto
            {
                ChannelName = display_name,
                AvatarUrl = logo,
                ChannelUrl = url,
                Platform = StreamPlatformType.Twitch,
            };
         }
    }
}
