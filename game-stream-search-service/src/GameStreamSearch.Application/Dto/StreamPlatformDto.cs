using System;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.ValueObjects
{
    public class StreamPlatformDto
    {
        public StreamPlatformType StreamPlatform { get; set; }
        public string StreamPlatformDisplayName => StreamPlatform.GetFriendlyName();
    }
}
