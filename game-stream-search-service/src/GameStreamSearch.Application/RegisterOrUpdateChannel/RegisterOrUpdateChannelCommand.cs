using System;
namespace GameStreamSearch.Application.RegisterOrUpdateChannel
{
    public class RegisterOrUpdateChannelCommand
    {
        public string ChannelName { get; init; }
        public string StreamPlatformName { get; init; }
    }
}
