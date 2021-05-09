using System;
namespace GameStreamSearch.Common
{
    public class ChannelNotFoundException : Exception
    {
        public ChannelNotFoundException(string streamPlatformName, string channelName)
        {
            StreamPlatformName = streamPlatformName;
            ChannelName = channelName;
        }

        public string StreamPlatformName { get; }
        public string ChannelName { get; }
    }
}
