﻿using System;

namespace GameStreamSearch.Application.ValueObjects
{
    public class PlatformChannel
    {
        public string ChannelName { get; init; }
        public StreamPlatform StreamPlatform { get; init; }
        public string AvatarUrl { get; init; }
        public string ChannelUrl { get; init; }

        public Channel ToChannel(DateTime dateRegistered)
        {
            return new Channel(ChannelName, StreamPlatform.PlatformId, dateRegistered, AvatarUrl, ChannelUrl);
        }
    }
}
