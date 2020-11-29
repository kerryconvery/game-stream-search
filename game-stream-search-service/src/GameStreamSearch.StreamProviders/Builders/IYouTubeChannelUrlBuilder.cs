using System;
namespace GameStreamSearch.StreamProviders.Builders
{
    public interface IYouTubeChannelUrlBuilder
    {
        string Build(string channelName);
    }
}
