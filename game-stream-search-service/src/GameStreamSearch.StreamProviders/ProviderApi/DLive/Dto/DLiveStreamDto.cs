using System;
using System.Collections.Generic;

namespace GameStreamSearch.StreamProviders.ProviderApi.DLive.Dto
{
    public class DLiveLiveStreamCreatorDto
    {
        public string displayName { get; set; }
        public string avatar { get; set; }
    }

    public class DLiveLiveStreamItemDto
    {
        public string title { get; set; }
        public int watchingCount { get; set; }
        public string thumbnailUrl { get; set; }
        public DLiveLiveStreamCreatorDto creator { get; set; }
    }

    public class DLiveLiveStreamsDto
    {
        public IEnumerable<DLiveLiveStreamItemDto> list { get; set; }
    }

    public class DLiveStreamDataDto
    {
        public DLiveLiveStreamsDto livestreams { get; set; }
    }

    public class DLiveStreamDto
    {
        public DLiveStreamDataDto data { get; set; }
    }
}