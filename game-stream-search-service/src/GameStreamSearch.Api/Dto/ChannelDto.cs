using System;
using GameStreamSearch.Domain.Enums;

namespace GameStreamSearch.Application.Dto
{
    public class ChannelDto
    {
        public string Name { get; init; }
        public StreamPlatformType StreamPlatform { get; init; }
        public DateTime DateRegistered { get; init; }
        public string StreamPlatformDisplayName => StreamPlatform.GetFriendlyName();
    }
}
