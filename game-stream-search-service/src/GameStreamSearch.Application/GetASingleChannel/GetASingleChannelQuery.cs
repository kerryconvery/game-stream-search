using System;
namespace GameStreamSearch.Domain.Queries
{
    public struct GetChannelQuery
    {
        public string platformName { get; init; }
        public string channelName { get; init; }
    }
}
