using System;
namespace GameStreamSearch.StreamProviders.Builders
{
    public class YouTubeWatchUrlBuilder : IYouTubeWatchUrlBuilder
    {
        private readonly string baseUrl;

        public YouTubeWatchUrlBuilder(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public string Build(string videoId)
        {
            return $"{baseUrl}/watch?v={videoId}";
        }
    }
}
