using System;
namespace GameStreamSearch.StreamProviders.Builders
{
    public class YouTubeChannelUrlBuilder : IYouTubeChannelUrlBuilder
    {
        private readonly string baseUrl;

        public YouTubeChannelUrlBuilder(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public string Build(string channelName)
        {
            return $"{baseUrl}/user/{channelName}";
        }
    }
}
