using System;
namespace GameStreamSearch.StreamProviders.Builders
{
    public interface IDLiveWatchUrlBuilder
    {
        string Build(string creatorDisplayName);
    }
}
