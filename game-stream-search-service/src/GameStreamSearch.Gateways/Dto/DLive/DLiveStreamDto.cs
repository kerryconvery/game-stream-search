using System;
using System.Collections.Generic;

namespace GameStreamSearch.Gateways.Dto.DLive
{
    public class DLiveStreamItemDto
    {
        public string title { get; set; }
        public int watchingCount { get; set; }
        public string thumbnailUrl { get; set; }
        public DLiveUserDto creator { get; set; }
    }

    public class DLiveStreamsDto
    {
        public IEnumerable<DLiveStreamItemDto> list { get; set; }
    }

    public class DLiveStreamDataDto
    {
        public DLiveStreamsDto livestreams { get; set; }
    }

    public class DLiveStreamDto
    {
        public DLiveStreamDataDto data { get; set; }
    }
}