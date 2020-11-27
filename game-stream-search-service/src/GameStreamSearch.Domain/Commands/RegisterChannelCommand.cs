using System;
using GameStreamSearch.Domain.Enums;

namespace GameStreamSearch.Domain.Commands
{
    public class RegisterChannelCommand
    {
        public string ChannelName { get; init; }
        public DateTime DateRegistered { get; init; }
        public StreamPlatformType StreamPlatform { get; init; }
        public string AvatarUrl { get; init; }
        public string ChanneelUrl { get; set; }
    }
}
