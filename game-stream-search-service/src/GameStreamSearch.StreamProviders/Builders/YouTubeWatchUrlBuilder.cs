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
            return string.Format("{0}/watch?v={1}", baseUrl, videoId);
        }
    }
}
