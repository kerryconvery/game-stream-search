using System;
namespace GameStreamSearch.StreamProviders.Builders
{
    public class DLiveWatchUrlBuilder : IDLiveWatchUrlBuilder
    {
        private readonly string baseUrl;

        public DLiveWatchUrlBuilder(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public string Build(string creatorDisplayName)
        {
            return $"{baseUrl}/{creatorDisplayName}";
        }
    }
}
