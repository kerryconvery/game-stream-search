using System;
namespace GameStreamSearch.StreamProviders.ProviderApi.Twitch.Dto.Kraken
{
    public class TwitchChannelDto
    {
        public string game { get; set; }
        public string display_name { get; set; }
        public string logo { get; set; }
        public string url { get; set; }
        public string status { get; set; }
    }
}
