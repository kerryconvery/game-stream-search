using System;
namespace GameStreamSearch.StreamProviders.Builders
{
    public interface IYouTubeWatchUrlBuilder
    {
        string Build(string videoId);
    }
}
