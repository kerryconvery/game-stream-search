using System;
using GameStreamSearch.Application.Enums;

namespace GameStreamSearch.Application.Dto
{
    public class StreamerDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public StreamingPlatform Platform { get; set; }
        public string StreamingPlatformDisplayName { get; set; }
        public DateTime DateRegistered { get; set; }
    }
}
